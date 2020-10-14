using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Glasswall.Administration.K8.TransactionEventApi.Common.Services;

namespace Glasswall.Administration.K8.TransactionEventApi.Business.Store
{
    public class AzureFileShare : IFileShare
    {
        private readonly ShareClient _shareClient;

        public AzureFileShare(ShareClient shareClient)
        {
            _shareClient = shareClient ?? throw new ArgumentNullException(nameof(shareClient));
        }

        public IAsyncEnumerable<string> ListAsync(IPathFilter pathFilter)
        {
            if (pathFilter == null) throw new ArgumentNullException(nameof(pathFilter));
            return RecurseDirectory(_shareClient.GetRootDirectoryClient(), pathFilter);
        }

        public Task<bool> ExistsAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Value must not be null or whitespace", nameof(path));
            return InternalExistsAsync(GetFileClient(path));
        }

        public Task<MemoryStream> DownloadAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Value must not be null or whitespace", nameof(path));
            return InternalDownloadAsync(path);
        }

        private async Task<MemoryStream> InternalDownloadAsync(string path)
        {
            var fileClient = GetFileClient(path);

            if (!await InternalExistsAsync(fileClient))
                return null;

            var ms = new MemoryStream();
            var file = await fileClient.DownloadAsync();
            await file.Value.Content.CopyToAsync(ms);
            return ms;
        }

        private static async Task<bool> InternalExistsAsync(ShareFileClient client)
        {
            return await client.ExistsAsync();
        }

        private ShareFileClient GetFileClient(string path)
        {
            var rootDirectory = _shareClient.GetRootDirectoryClient();
            return rootDirectory.GetFileClient(path);
        }

        private static async IAsyncEnumerable<string> RecurseDirectory(ShareDirectoryClient directory, IPathFilter pathFilter)
        {
            var directoryPath = directory.Path;
            await foreach (var item in directory.GetFilesAndDirectoriesAsync())
            {
                var nextPath = $"{directoryPath}{(directoryPath == "" ? "" : "/")}{item.Name}";
                var nextPathAction = pathFilter.DecideAction(nextPath);

                if (item.IsDirectory && nextPathAction == PathAction.Recurse)
                {
                    var subDirectory = directory.GetSubdirectoryClient(item.Name);

                    await foreach (var subItem in RecurseDirectory(subDirectory, pathFilter)) yield return subItem;
                }
                else if (nextPathAction == PathAction.Collect)
                    yield return nextPath;
            }
        }
    }
}
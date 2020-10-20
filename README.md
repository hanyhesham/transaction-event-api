# transaction-event-api
Glasswall Transaction API

# Setup

## Pre-requisites

### API

- Docker desktop with kubernetes
- aspnet core 3.1 or later
- Visual studio 

### Swagger

- vscode
- node/npm

# How to

## Debug the API locally

- Open TransactionEventHandler.sln
- Ensure docker-compose is the startup project
- Start docker compose
- API is reachable at http://localhost:32769 and https://localhost:32768

## Run Swagger Page locally

- Ensure API is running if swagger is to target your local API
- Open vscode
- Open a terminal in gh-page
- run `npm i` to install packages
- run `npm start` to launch the swagger page
- This should launch a browser at "http://localhost:3000/transaction-event-api"


test

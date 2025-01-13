
# MailCrafter - Core Layer (Domain, Services, Repositories)

This repository contains the **Core Layer** of the MailCrafter system, built as a .NET library. The Core Layer provides the essential business logic, domain models, and repositories for managing data operations, ensuring consistent logic throughout the system.

## Features
- **Domain Models**: Represents core entities such as `EmailCampaign`, `Recipient`, and `Template`.
- **Business Services**: Provides the business logic for tasks like email personalization and campaign management.
- **Repositories**: Implements data access logic for interacting with **MongoDB** storage.

## Architecture
- The Core Layer is designed as a **.NET library** with no UI or API.
- Reusable business logic is shared between the **Application Layer** and **Worker Nodes**.
- Interfaces with the **MongoDB** storage layer to store and retrieve campaign data.

## Setup and Installation
### Prerequisites:
- **.NET 8 or later**
- **MongoDB** instance

### Installation Steps:
1. Clone this repository.
   ```bash
   git clone https://github.com/CodieGlot/MailCrafter.Core.git
   ```
2. Restore dependencies.
   ```bash
   dotnet restore
   ```

## Usage
- This layer is referenced by both the **Application Layer** and **Worker Nodes**. It should not be run independently.
- Use the services and repositories to interact with the domain and data models.

## License
MIT License. See LICENSE file for details.

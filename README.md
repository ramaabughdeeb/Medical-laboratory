# Laboratory Desktop App — Demo Workflow

Windows Forms / .NET 8 desktop UI for a laboratory workflow. All data is held in static in-memory lists and resets when the app closes. There is no database, backend, API, HTTP call, or external service.

## Workflow
1. **Patients**: register a patient with name, phone, gender, birth date and notes.
2. **Test Catalog**: browse the available demo tests.
3. **Test Orders**: create an order for a registered patient and select one or more catalog tests.
4. **Samples**: register or update a sample and barcode for an order; update collection/processing status.
5. **Results**: select an order and test, enter parameter values, and mark them Draft, Reviewed, or Approved.
6. **Reports**: view order status/result counts and export a demo CSV summary.

## Run
Open `LaboratoryDesktopApp.csproj` in Visual Studio 2022 with the **.NET desktop development** workload and .NET 8 SDK installed. On Windows, run the project (F5). Alternatively, from the folder containing the `.csproj`, run `dotnet run`.

The project references FontAwesome.Sharp for the existing navigation package. The current sample data and all entered patient/test information are fictional. This is a front-end learning prototype, not a clinical system; do not enter real patient data or use its demo ranges/results for medical decisions.

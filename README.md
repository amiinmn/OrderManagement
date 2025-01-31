# OrderManagement

## Prerequisites
- .NET SDK (download it from [here](https://dotnet.microsoft.com/download))
- Git (optional, for cloning the repository)

## Installation
1. **Clone the repository**
    ```bash
    git clone https://github.com/amiinmn/OrderManagement.git
    cd OrderManagement
    ```

2. **Restore dependencies**
    ```bash
    dotnet restore
    ```

3. **Build the project**
    ```bash
    dotnet build
    ```

4. **Run the project**
    ```bash
    dotnet run
    ```

## APIs
1. **Order List**
   #### Request: `http://localhost:5245/api/orders?keywords=&orderDate=`
   #### Response:
   ```json
   [
    {
        "salesOrder": "SO_202501316",
        "orderDate": "31/01/2025",
        "customer": "PROFES"
    },
    {
        "salesOrder": "SO_202501315",
        "orderDate": "31/01/2025",
        "customer": "TITAN"
    }
   ]
   ```
   

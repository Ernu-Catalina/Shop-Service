Shop Service

The Shop Service manages in-game items that characters can purchase and use during the game.
It works together with the Character Service (for gold balance) and the Inventory Service (to store purchased items).

---

Endpoints

1. List Shop Items

GET /api/shop/items

Returns all items available in the shop.

Response (200 OK):

[
{
"id": 1,
"title": "Garlic",
"description": "Protects from Vampire attack (1 use).",
"price": 50,
"durability": 1,
"rolesVisible": ["Citizen", "Doctor", "Detective"]
},
{
"id": 2,
"title": "Healing Herbs",
"description": "Doctor equipment (3 uses). Needed to save players.",
"price": 80,
"durability": 3,
"rolesVisible": ["Doctor"]
}
]

---

2. Purchase Item

POST /api/shop/purchase

Attempts to buy an item for a character.
Checks character’s gold balance via Character Service, and if successful, adds the item to Inventory Service.

Payload:

{
"characterId": 101,
"itemId": 1,
"quantity": 1
}

Success Response (200 OK):

{
"purchaseId": 2001,
"itemsAdded": [
{
"inventoryItemId": 501,
"itemId": 1,
"remainingDurability": 1
}
],
"newBalance": 30
}

Failure Response (400 Bad Request):

{
"error": "Insufficient funds"
}

---

How to Run the Service

1. Make sure you have .NET 10 SDK installed: https://aka.ms/dotnet-download
2. Open a terminal or PowerShell in the project root directory (where ShopService.csproj is located).
3. Restore dependencies:

   dotnet restore

4. Build the project:

   dotnet build

5. Run the service on localhost port 5000:

   dotnet run --urls http://localhost:5000

6. The API will be accessible at:

   http://localhost:5000/api/shop/items
   http://localhost:5000/api/shop/purchase

Optional: You can also use the Python build_and_run.py script to automate steps 3-5:

python build_and_run.py

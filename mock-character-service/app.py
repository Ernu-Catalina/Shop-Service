from flask import Flask, jsonify, request

app = Flask(__name__)

# Simple in-memory store for testing
characters = {
    123: {
        "role_id": 3,
        "currency": 200,
        "inventory": []
    },
    456: {
        "role_id": 2,
        "currency": 40,
        "inventory": [14]  # has Magnifying Glass
    }
}

@app.route('/character/<int:character_id>/role', methods=['GET'])
def get_role(character_id):
    c = characters.get(character_id)
    if not c:
        return jsonify({"error": "not found"}), 404
    return jsonify({"role_id": c["role_id"]})

@app.route('/character/<int:character_id>/currency', methods=['GET'])
def get_currency(character_id):
    c = characters.get(character_id)
    if not c:
        return jsonify({"currency_amount": 0}), 404
    return jsonify({"currency_amount": c["currency"]})

@app.route('/character/<int:character_id>/inventory/add', methods=['POST'])
def add_inventory(character_id):
    payload = request.get_json() or {}
    item_id = payload.get("item_id")
    if item_id is None:
        return jsonify({"success": False, "message": "missing item_id"}), 400
    c = characters.get(character_id)
    if not c:
        return jsonify({"success": False, "message": "character not found"}), 404
    # Deduct a mock price
    price_map = {1:50,2:60,3:80,4:100,5:60,6:80,7:100,8:50,9:60,10:100,11:60,12:80,13:60,14:80}
    price = price_map.get(item_id, 50)
    if c["currency"] < price:
        return jsonify({"success": False, "message": "not enough currency"}), 400
    c["currency"] -= price
    c["inventory"].append(item_id)
    return jsonify({"success": True, "message": "item added", "new_currency": c["currency"]})

@app.route('/character/<int:character_id>/inventory', methods=['GET'])
def get_inventory(character_id):
    c = characters.get(character_id)
    if not c:
        return jsonify({"error": "not found"}), 404
    return jsonify({"inventory": c["inventory"]})

@app.route('/character/<int:character_id>/inventory/use', methods=['POST'])
def use_inventory(character_id):
    payload = request.get_json() or {}
    item_id = payload.get("item_id")
    c = characters.get(character_id)
    if not c:
        return jsonify({"success": False, "message": "character not found"}), 404
    if item_id in c["inventory"]:
        c["inventory"].remove(item_id)
        return jsonify({"success": True, "message": "used"})
    return jsonify({"success": False, "message": "item not in inventory"}), 400

if __name__ == "__main__":
    print("Mock Character Service running on 0.0.0.0:4002")
    app.run(host="0.0.0.0", port=4002)

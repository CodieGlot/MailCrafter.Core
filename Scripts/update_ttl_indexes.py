import json
import os
from pymongo import MongoClient, ASCENDING

APPSETTINGS_PATH = "C:\MailCrafter\Development\Core\appsettings.Development.json"

def load_config():
    if not os.path.exists(APPSETTINGS_PATH):
        raise FileNotFoundError(f"Config file not found: {APPSETTINGS_PATH}")

    with open(APPSETTINGS_PATH, "r", encoding="utf-8") as file:
        config = json.load(file)
    
    connection_uri = config.get("MongoDB", {}).get("ConnectionURI")
    database_name = config.get("MongoDB", {}).get("DatabaseName")

    if not connection_uri or not database_name:
        raise ValueError("MongoDB configuration is missing in config file")

    return connection_uri, database_name

def update_ttl_index(collection):
    index_name = "ExpiresAt_1"

    # Check existing indexes
    existing_indexes = list(collection.list_indexes())
    if any(index["name"] == index_name for index in existing_indexes):
        print(f"TTL index already exists for {collection.name}, skipping...")
        return

    # Create TTL index
    collection.create_index([("ExpiresAt", ASCENDING)], expireAfterSeconds=0)
    print(f"TTL index created for {collection.name}.")

def main():
    try:
        connection_uri, database_name = load_config()
        client = MongoClient(connection_uri)
        db = client[database_name]

        collections = ["EmailTemplates", "CustomGroups"]

        for collection_name in collections:
            collection = db[collection_name]
            update_ttl_index(collection)

        print("TTL index update completed successfully.")
    except Exception as e:
        print(f"Error: {e}")

if __name__ == "__main__":
    main()

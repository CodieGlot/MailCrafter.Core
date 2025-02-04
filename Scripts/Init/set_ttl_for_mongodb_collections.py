import pymongo
from datetime import datetime, timedelta

# Connect to MongoDB
client = pymongo.MongoClient("mongodb://localhost:27017/")
db = client["your_database_name"]

# List of collection names to update
collections = ["EmailTemplates", "CustomGroups"]

# Function to set TTL for documents in the collection
def set_ttl_for_collection(collection_name):
    collection = db[collection_name]

    # Create TTL index on the ExpiresAt field (with expireAfterSeconds set to 0 to enable TTL)
    collection.create_index([("ExpiresAt", pymongo.ASCENDING)], expireAfterSeconds=0)

    print(f"TTL index created for the ExpiresAt field in {collection_name}.")

# Loop through all the collections and set TTL
for collection_name in collections:
    set_ttl_for_collection(collection_name)

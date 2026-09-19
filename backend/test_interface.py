from npc_engine.interface import NPCSession

session = NPCSession()

print("Starting conversation...")
print(session.start())

while True:
    user_input = input("You: ")
    response = session.step(user_input)
    print("NPC:", response)

    if response["done"]:
        break

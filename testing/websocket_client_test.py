import asyncio
import json
import websockets
import sys
import uuid

# Define standard terminal colors for clean outputs
CLR_CYAN = "\033[96m"
CLR_GREEN = "\033[92m"
CLR_YELLOW = "\033[93m"
CLR_RED = "\033[91m"
CLR_RESET = "\033[0m"
CLR_BOLD = "\033[1m"

SESSION_ID = f"sandbox-{uuid.uuid4()}"
WS_URI = f"ws://127.0.0.1:8000/ws/negotiate/{SESSION_ID}"

# Event to coordinate turns: signals that a server response has arrived
response_event = asyncio.Event()

async def receive_messages(websocket):
    """Listens continuously for background events (like TTS audio generation completion)"""
    try:
        async for message in websocket:
            data = json.loads(message)
            msg_type = data.get("type")
            
            if msg_type == "audio_ready":
                print(f"\n{CLR_BOLD}{CLR_GREEN}[ASYNC TTS EVENT] Audio synthesized successfully!{CLR_RESET}")
                print(f"  URL: {CLR_CYAN}{data.get('audio_url')}{CLR_RESET}\n")
            elif msg_type == "welcome":
                print(f"\n{CLR_BOLD}{CLR_YELLOW}🤝 NEW BUYER SPOTTED IN BAZAAR (session_id={data.get('session_id')}){CLR_RESET}")
                print(f"{CLR_BOLD}{CLR_RED}Buyer:{CLR_RESET} \"{data.get('npc_text')}\"")
                print(f"  Tone: {data.get('tone')} | Emotion: {data.get('emotion')}")
                response_event.set()  # Signal welcome received
            elif msg_type == "text_response":
                print(f"\n{CLR_BOLD}{CLR_RED}Buyer:{CLR_RESET} \"{data.get('npc_text')}\"")
                print(f"  Action: {CLR_CYAN}{data.get('action')}{CLR_RESET} | Price: {data.get('price')} varahas | Quantity: {data.get('quantity')}g")
                print(f"  Tone: {data.get('tone')} | Emotion: {data.get('emotion')}")
                if data.get("done"):
                    print(f"\n{CLR_BOLD}{CLR_GREEN}🎉 Negotiation concluded successfully.{CLR_RESET}")
                response_event.set()  # Signal response received
            elif msg_type == "error":
                print(f"{CLR_BOLD}{CLR_RED}[ERROR] Server sent error: {data.get('message')}{CLR_RESET}")
                response_event.set()
    except websockets.exceptions.ConnectionClosed:
         pass
    except Exception as e:
         print(f"[ERROR] Exception in message receiver: {e}")

async def run_mock_gameplay():
    print(f"{CLR_BOLD}{CLR_CYAN}============================================================{CLR_RESET}")
    print(f" {CLR_BOLD}👑 STARTING WEBSOCKET INTEGRATION TEST FOR UNITY VR {CLR_RESET}")
    print(f"{CLR_BOLD}{CLR_CYAN}============================================================{CLR_RESET}")
    print(f"Connecting to backend WebSocket at: {CLR_CYAN}{WS_URI}{CLR_RESET}...")
    
    try:
        async with websockets.connect(WS_URI, open_timeout=30) as websocket:
            # Spawn the concurrent background receiver task
            receiver_task = asyncio.create_task(receive_messages(websocket))
            
            # Wait for welcome message
            await response_event.wait()
            response_event.clear()
            
            # Step 1: Tell the buyer what quantity we have (Establish standard spice)
            print(f"\n{CLR_BOLD}{CLR_CYAN}You (Seller):{CLR_RESET} \"I have 1 Veesai (~1.4 kg) of Cinnamon available today.\"")
            await websocket.send(json.dumps({"player_input": "I have 1 Veesai"}))
            
            # Wait for response before proceeding
            await response_event.wait()
            response_event.clear()
            
            # Step 2: Propose counter offer price
            print(f"\n{CLR_BOLD}{CLR_CYAN}You (Seller):{CLR_RESET} \"The price is 150 varahas\"")
            await websocket.send(json.dumps({"player_input": "The price is 150 varahas"}))
            
            # Wait for response before proceeding
            await response_event.wait()
            response_event.clear()
            
            # Step 3: Concede slightly and see if the buyer overshoots
            print(f"\n{CLR_BOLD}{CLR_CYAN}You (Seller):{CLR_RESET} \"ok let's meet at 115\"")
            await websocket.send(json.dumps({"player_input": "ok let's meet at 115"}))
            
            # Wait for response before proceeding
            await response_event.wait()
            response_event.clear()
            
            # Step 4: Settle the deal
            print(f"\n{CLR_BOLD}{CLR_CYAN}You (Seller):{CLR_RESET} \"so lets call it a deal\"")
            await websocket.send(json.dumps({"player_input": "so lets call it a deal"}))
            
            # Wait for final response before closing
            await response_event.wait()
            response_event.clear()
            
            # Wait a moment for background TTS to wrap up the final deal audio
            await asyncio.sleep(2.0)
            
            # Clean up the receiver task
            receiver_task.cancel()
            
    except Exception as e:
        print(f"\n{CLR_BOLD}{CLR_RED}[FATAL ERROR] Failed to connect or execute WebSocket flow: {e}{CLR_RESET}")
        print("Ensure you started the FastAPI backend server first (uvicorn api:app --host 127.0.0.1 --port 8000)")
        sys.exit(1)

    print(f"\n{CLR_BOLD}{CLR_CYAN}============================================================{CLR_RESET}")
    print(f" {CLR_BOLD}{CLR_GREEN}✓ WEBSOCKET & ASYNC TTS RUNNER COMPLETED SUCCESSFULY!{CLR_RESET}")
    print(f"{CLR_BOLD}{CLR_CYAN}============================================================{CLR_RESET}\n")

if __name__ == "__main__":
    try:
        asyncio.run(run_mock_gameplay())
    except KeyboardInterrupt:
        print("\nTest client shutdown.")

import os
import re

scenes = {
    "SpicesIntro": r"g:\Users\chitr\Desktop\Capstone\AI-Enabled-Storytelling-VR\unity\StorytellingVR\Assets\_Project\Scenes\SpicesIntro.unity",
    "Transcation_Tutorial": r"g:\Users\chitr\Desktop\Capstone\AI-Enabled-Storytelling-VR\unity\StorytellingVR\Assets\_Project\Scenes\Transcation_Tutorial.unity",
    "MainScene1": r"g:\Users\chitr\Desktop\Capstone\AI-Enabled-Storytelling-VR\unity\StorytellingVR\Assets\_Project\Scenes\MainScene1.unity",
    "MainScene1_PreVRBackup": r"g:\Users\chitr\Desktop\Capstone\AI-Enabled-Storytelling-VR\unity\StorytellingVR\Assets\_Project\Scenes\MainScene1_PreVRBackup.unity",
}

def audit_main_camera(filepath, scene_name):
    print(f"\n=================== {scene_name} ===================")
    with open(filepath, 'r', encoding='utf-8', errors='ignore') as f:
        content = f.read()
        
    docs = content.split('--- !u!')
    
    go_id = None
    components = []
    
    # 1. Find GameObject named "Main Camera"
    for doc in docs:
        if not doc.strip():
            continue
        lines = doc.splitlines()
        header = lines[0]
        match = re.match(r'(\d+) &(-?\d+)', header)
        if not match:
            continue
        class_id, file_id = match.group(1), match.group(2)
        doc_body = '\n'.join(lines[1:])
        
        if class_id == '1': # GameObject
            name_match = re.search(r'm_Name: (.*)', doc_body)
            if name_match and name_match.group(1).strip() == "Main Camera":
                go_id = file_id
                # Parse components list
                comp_matches = re.findall(r'- component: {fileID: (-?\d+)}', doc_body)
                components = comp_matches
                print(f"Main Camera GameObject ID: {go_id}")
                break
                
    if not go_id:
        print("Main Camera GameObject not found.")
        return
        
    # 2. Inspect each component
    for comp_id in components:
        # Find doc with this file_id
        for doc in docs:
            if not doc.strip():
                continue
            lines = doc.splitlines()
            header = lines[0]
            match = re.match(r'(\d+) &(-?\d+)', header)
            if not match:
                continue
            class_id, file_id = match.group(1), match.group(2)
            if file_id == comp_id:
                doc_body = '\n'.join(lines[1:])
                # Print type and enabled status if available
                enabled_match = re.search(r'm_Enabled: (\d+)', doc_body)
                enabled_str = f" | Enabled: {enabled_match.group(1)}" if enabled_match else ""
                print(f"  Component class: {class_id} (ID: {comp_id}){enabled_str}")
                # If script, print script guid
                if class_id == '114':
                    script_match = re.search(r'm_Script: {fileID: \d+, guid: ([a-f0-9]+), type: \d+}', doc_body)
                    if script_match:
                        print(f"    Script GUID: {script_match.group(1)}")
                break

def main():
    for name, path in scenes.items():
        audit_main_camera(path, name)

if __name__ == '__main__':
    main()

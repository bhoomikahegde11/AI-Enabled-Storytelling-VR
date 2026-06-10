import os
import re

ovrmanager_guid = "7e933e81d3c20c74ea6fdc708a67e3a5"
gamemanager_guid = "1be4528c21d6463429bb2b1af729e2f0"
camera_comp_id = "330585545"

scenes_to_fix_origin = [
    r"g:\Users\chitr\Desktop\Capstone\AI-Enabled-Storytelling-VR\unity\StorytellingVR\Assets\_Project\Scenes\MainScene1.unity",
    r"g:\Users\chitr\Desktop\Capstone\AI-Enabled-Storytelling-VR\unity\StorytellingVR\Assets\_Project\Scenes\MainScene1_PreVRBackup.unity",
    r"g:\Users\chitr\Desktop\Capstone\AI-Enabled-Storytelling-VR\unity\StorytellingVR\Assets\_Project\Scenes\Transcation_Tutorial.unity",
    r"g:\Users\chitr\Desktop\Capstone\AI-Enabled-Storytelling-VR\unity\StorytellingVR\Assets\_Project\Scenes\TraderLightingFix.unity"
]

bootstrap_scene = r"g:\Users\chitr\Desktop\Capstone\AI-Enabled-Storytelling-VR\unity\StorytellingVR\Assets\_Project\Scenes\Bootstrap.unity"
tutorial_scene = r"g:\Users\chitr\Desktop\Capstone\AI-Enabled-Storytelling-VR\unity\StorytellingVR\Assets\_Project\Scenes\Transcation_Tutorial.unity"

def fix_file_origins_and_camera():
    # 1. Fix _trackingOriginType in the target scenes
    for filepath in scenes_to_fix_origin:
        if not os.path.exists(filepath):
            print(f"File not found: {filepath}")
            continue
            
        print(f"\nProcessing origin fix in {os.path.basename(filepath)}...")
        with open(filepath, 'r', encoding='utf-8', errors='ignore', newline='') as f:
            content = f.read()
            
        docs = content.split('--- !u!')
        modified = False
        
        for idx, doc in enumerate(docs):
            if not doc.strip():
                continue
            lines = doc.splitlines()
            header = lines[0]
            match = re.match(r'(\d+) &(-?\d+)', header)
            if not match:
                continue
            class_id = match.group(1)
            
            doc_body = '\n'.join(lines[1:])
            if class_id == '114' and ovrmanager_guid in doc_body:
                # OVRManager component
                new_lines = []
                for line in lines:
                    if '_trackingOriginType:' in line:
                        if '_trackingOriginType: 1' in line:
                            line = line.replace('_trackingOriginType: 1', '_trackingOriginType: 0')
                            modified = True
                            print("  Changed _trackingOriginType from 1 (Floor Level) to 0 (Eye Level)")
                    new_lines.append(line)
                docs[idx] = '\r\n'.join(new_lines) if '\r\n' in doc else '\n'.join(new_lines)
                
        if modified:
            new_content = '--- !u!'.join(docs)
            with open(filepath, 'w', encoding='utf-8', newline='') as f:
                f.write(new_content)
            print("  Successfully saved scene file.")
        else:
            print("  No origin fix changes needed.")

    # 2. Disable Main Camera component in Transcation_Tutorial.unity
    if os.path.exists(tutorial_scene):
        print(f"\nProcessing Main Camera component disable in {os.path.basename(tutorial_scene)}...")
        with open(tutorial_scene, 'r', encoding='utf-8', errors='ignore', newline='') as f:
            content = f.read()
            
        docs = content.split('--- !u!')
        modified = False
        
        for idx, doc in enumerate(docs):
            if not doc.strip():
                continue
            lines = doc.splitlines()
            header = lines[0]
            match = re.match(r'(\d+) &(-?\d+)', header)
            if not match:
                continue
            class_id, file_id = match.group(1), match.group(2)
            
            if class_id == '20' and file_id == camera_comp_id:
                # Camera component
                new_lines = []
                for line in lines:
                    if 'm_Enabled:' in line:
                        if 'm_Enabled: 1' in line:
                            line = line.replace('m_Enabled: 1', 'm_Enabled: 0')
                            modified = True
                            print("  Changed m_Enabled from 1 to 0 on desktop Main Camera component")
                    new_lines.append(line)
                docs[idx] = '\r\n'.join(new_lines) if '\r\n' in doc else '\n'.join(new_lines)
                
        if modified:
            new_content = '--- !u!'.join(docs)
            with open(tutorial_scene, 'w', encoding='utf-8', newline='') as f:
                f.write(new_content)
            print("  Successfully saved scene file.")
        else:
            print("  No camera component changes needed.")

    # 3. Update GameManager scene list in Bootstrap.unity
    if os.path.exists(bootstrap_scene):
        print(f"\nProcessing GameManager scene list update in {os.path.basename(bootstrap_scene)}...")
        with open(bootstrap_scene, 'r', encoding='utf-8', errors='ignore', newline='') as f:
            content = f.read()
            
        docs = content.split('--- !u!')
        modified = False
        
        for idx, doc in enumerate(docs):
            if not doc.strip():
                continue
            lines = doc.splitlines()
            header = lines[0]
            match = re.match(r'(\d+) &(-?\d+)', header)
            if not match:
                continue
            class_id = match.group(1)
            
            doc_body = '\n'.join(lines[1:])
            if class_id == '114' and gamemanager_guid in doc_body:
                # GameManager component
                new_lines = []
                in_scenes = False
                for line in lines:
                    if 'scenes:' in line:
                        in_scenes = True
                    elif in_scenes and not line.strip().startswith('-'):
                        in_scenes = False
                    
                    if in_scenes and '- MainScene1' in line and not '- MainScene1_PreVRBackup' in line:
                        line = line.replace('- MainScene1', '- MainScene1_PreVRBackup')
                        modified = True
                        print("  Changed scene list entry: MainScene1 -> MainScene1_PreVRBackup")
                    new_lines.append(line)
                docs[idx] = '\r\n'.join(new_lines) if '\r\n' in doc else '\n'.join(new_lines)
                
        if modified:
            new_content = '--- !u!'.join(docs)
            with open(bootstrap_scene, 'w', encoding='utf-8', newline='') as f:
                f.write(new_content)
            print("  Successfully saved scene file.")
        else:
            print("  No scene list changes needed.")

if __name__ == '__main__':
    fix_file_origins_and_camera()

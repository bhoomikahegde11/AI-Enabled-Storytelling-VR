import re

filepath = r"g:\Users\chitr\Desktop\Capstone\AI-Enabled-Storytelling-VR\unity\StorytellingVR\Assets\_Project\Scenes\Bootstrap.unity"

def main():
    with open(filepath, 'r', encoding='utf-8', errors='ignore') as f:
        content = f.read()
        
    docs = content.split('--- !u!')
    for doc in docs:
        if 'm_Script: {fileID: 11500000, guid: 1be4528c21d6463429bb2b1af729e2f0' in doc: # GameManager script
            print("================ GameManager Block ================")
            print(doc)
            print("===================================================")

if __name__ == '__main__':
    main()

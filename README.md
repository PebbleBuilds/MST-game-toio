# Setup
To clone and run this repo on your own Mac device:
- Install Unity 2021.3.39f1 through the Unity Hub
- Install UniTask (https://github.com/Cysharp/UniTask?tab=readme-ov-file#upm-package)
- Install Netcode for GameObjects through the Package Manager on Unity (https://docs-multiplayer.unity3d.com/netcode/current/installation/ - Version 1.9.1 should be OK)
- Add the MacBLE plugin for your MacOS version (https://github.com/morikatron/toio-sdk-for-unity/blob/main/docs_EN/usage_macble.md#how-to-build-the-bundle-file-on-your-own-pc)
- Install this extra multiplayer plugin https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop?path=/Packages/com.unity.multiplayer.samples.coop

# Running the Code
You don't have to build - you can just run in the editor. Use the old Mac as the Host, and the new Mac as the Client. Make sure ifconfig on the old Mac is the same as the IP address inputted into both Unity Editors. 
(current status. if you Host and Client on the same computer, it works - on both computers. If you Host and Client on different computers, you get an error.)

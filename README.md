# Setup
To clone and run this repo on your own Mac device:
- Install Unity 2021.3.0f1 through the Unity Hub
- Install UniTask (https://github.com/Cysharp/UniTask?tab=readme-ov-file#upm-package)
- Install Netcode for GameObjects through the Package Manager on Unity (https://docs-multiplayer.unity3d.com/netcode/current/installation/ - Version 1.6.0 should be OK)
- Add the MacBLE plugin for your MacOS version (https://github.com/morikatron/toio-sdk-for-unity/blob/main/docs_EN/usage_macble.md#how-to-build-the-bundle-file-on-your-own-pc)
- Turn off Burst AOT compilation

# Running the Code
Right now, the code only runs properly in the editor. Make sure the IP address in both editors is set to that of the host computer. When starting the code, try to make sure the swarms are far away from each other. Also, try turning on and connecting one swarm, and then turning on the second swarm only once the connection is complete on the first swarm. 

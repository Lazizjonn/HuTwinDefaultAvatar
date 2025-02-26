
Packages used:

- 🛠️ Package "com.meta.xr.sdk.all" v69.0.0.
- 🛠️ Package "com.meta.movement" v69.0.0 (git).
- 🛠️ Package "com.unity.netcode.gameobjects" v2.2.0.

Import below samples from menu: Window -> Package Manager:
- ⬇️ Import "Starter Asset" and "Hands Interaction Demo" samples in "com.unity.xr.interaction.toolkit" v3.0.7 package.
- ⬇️ Import "Hand Visualizer" sample in "com.unity.xr.hands" v1.5.0 package.
- ⬇️ Import "Face Tracking Samples" and "Body Tracking Samples" from "com.meta.movement" v69.0.0 package.

🚨 Check this "CorrectivesFace.cs" script (in "Packages\com.meta.movement\Runtime\Scripts\Tracking\FaceTrackingData\" directory): 
- it's Update() method should be empty;
- it should have UpdateExpressionWeightFromRemote(...) and PrepareRemoteExpressionWeights() methods.


Note 🪧: Build an apk file with "it.cagliari.HuTwinDefaultAvatar" package name in Build Profiles setting panel.

Logged expression files (open and process in Excel):
- 📃 "bone_log_yyyy_MM_dd_HH-mm-ss.csv"
- 📃 "face_log_yyyy_MM_dd_HH-mm-ss.csv"
    - if android app:               💾 in "Storage\Android\data\it.cagliari.HuTwinDefaultAvatar\files\..."
    - if play mode or exe mode:     💾 in "C:\Users\<User-name>\AppData\LocalLow\cagliary\HuTwinDefaultAvatar\..."
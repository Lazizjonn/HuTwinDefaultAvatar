
Packages:
- Package "com.meta.xr.sdk.all" v69.0.0.
- Package "com.meta.movement" v69.0.0 (git).
- Package "com.unity.netcode.gameobjects" v2.2.0.

Imports:
- Import "Starter Asset" and "Hands Interaction Demo" samples in "com.unity.xr.interaction.toolkit" v3.0.7 package.
- Import "Hand Visualizer" sample in "com.unity.xr.hands" v1.5.0 package.

Added HighFidelity movement avatar in this commit. Face and body movement works.
But avatar face expression is set all to avatars taking from local player and doesn't send to network in Multiplayer mode.


Note: Build an apk file with "it.cagliari.HuTwinDefaultAvatar" package name in Build Profiles setting panel.

Todo: RollingPin and Bowl with tomato has problem, fix it!
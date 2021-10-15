# New App Checklist

These are all the things I used to forget to do when I started a new project.
So I wrote them down and here we are.

### Platform
* Change platform to Android in Build Settings.
* Add Open Scene to build.

### Packages
v4.0.8 works for all of these, but 4.1.x broke some custom shaders and created
a nightmare to untangle. (That could very well be poorly written custom
shaders.)
* AR Foundation
* AR Core XR Plugin
* AR Kit XR Plugin
* Don't install AR Kit Facetracking -- App Store will reject apps with
this package unless they use the front camera.

### Settings
* Project Settings -> XR Plugin Management -> Check ARCore for Android and
ARKit for iOS
* Project Settings -> Quality
    * Shadow resolution: High Resolution
    * Shadow Distance: 8
* Player Settings:
    * Add descriptions for camera, microphone, and/or location usage.
    * Company/project name (com.ingrate.appTitle)
    * Add default Ingrate icon
    * Splash image
        * Add Ingrate logo
        * Change BG color to white
        * Change other setting to dark on light
    * Android:
        * Scripting backend: should be IL2CPP, not Mono
        * Target architectures: ARMv7 and ARM 64
        * Set a min/target API level (29 works as of January 2021)
    * iOS
        * Signing team ID: add Apple ID email
        * Check "requires ARKit Support"
        * Set minimum iOS version

### Scene/Hierarchy 
* Remember to add an Event System to the scene. (When you can't figure out why
none of your UI buttons work: this is why.)
* AR Session Origin - set appropriate plane detection mode.

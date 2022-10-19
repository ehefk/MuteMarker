# Update Note:

This project uses WD-on currently.  When I created this I didn't know much about what that meant, it just worked.

An updated version is forthcoming that will have a toggle in the installer.  That's all!  


# Mute Marker for VRChat
Inspired by what I consider major missing features of currently existing AV3 markers, I created my own version, designed to make communicating as a mute in VRChat a little easier.

I've been asked more than a few times where I got my marker, but most people aren't familiar enough with Unity so I made this installer.

Hopefully this helps some people out!

This was my first Unity editor extension.  If there are any problems, feature requests, etc -- please let me know.  Thanks!

## Now with 1-click installer script:

- Places the markers on the tip of your index finger
- Uses minimal parameters (9 or 10, depending if you want the ambidextrous option)
- You can set color, emission, size and ink lifetime
- Update or remove marker at any time.  Cleanly removes or updates components on the avatar descriptor


### Install instructions
[Click here for the the latest download link](https://github.com/ehefk/MuteMarker/releases/tag/v1.0.0)

- Drag the MuteMarker prefab into your scene
- Drag your Avatar into the Avatar slot
- Change the defaults if you wish
- (Optional) Drag the marker handles to adjust the position on your index finger
- Click apply
- Enjoy!


### Usage instructions
- Navigate to the Mute Marker menu and pick your prefered gesture
- Select which marker you want active.  Both is fine.
- Use the 'point' gesture on the opposite hand of your marker to pick up and move the canvas

### Important Notes: 
- **The canvas pickup isn't perfect, but it does work after you repeat the finger-point gesture to reset the space orientation**
  - The mechanism for this is a nested world-canvas.  In the next update I want to add a second nested canvas so that you can flip your writing 180 degrees.  This should let you write normally, then turn it around for the camera or people in front of you to easily read.
- **If you have more than 1 scene with the same exact avatar name, you'll have to use "in-place" mode until the next update**


**Install Notes:**
- After hitting apply, the marker objects will be placed near the tip of your index finger -- if you want it to be all the way on the tip, just grab the X handle and drag it an inch or two.
- I plan at least one update (before vrchat updates force one) to add the ability to disable the canvas pick-up as well as enable a canvas-rotate gesture. (will likely be the victory gesture on the same hand as canvsa pick-up)


## Credits:

- Killfrenzy -- provided the animation layers that properly control the fist gestures
- Hai -- inspiration for the robust unity editor install script
- Phasedragon from the VRChat discord -- answering my pepega-brained Unity questions


![alt install pic](https://github.com/ehefk/MuteMarker/blob/main/installation.png)

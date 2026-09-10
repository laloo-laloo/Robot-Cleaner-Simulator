Particle System VFX - Vol. 1
============================

These are Unity ParticleSystem prefabs. Each one emits and simulates at
runtime, so it scales, tints and re-times without anything being redrawn.
Nothing here is a baked effect: none of the 5 effects is a recording of a
frame sequence, which is what lets you change one rather than only play it.

The particles are drawn with the masks in Textures/ through the pack's own
unlit particle shader, so the same materials draw in Built-in, URP and HDRP.


What is in the pack
-------------------
  TorchFire      looping   TorchFire
  SparkleBurst   one-shot  SparkleBurst
  SmokePlume     looping   SmokePlume
  ImpactBurst    one-shot  ImpactBurst
  EnergyVortex   looping   EnergyVortex

Start with the demo scene
-------------------------
Demo/ParticleSystemVFXVol1 Demo.unity has every effect laid out on a ground plane with
its name beside it. Press Play to watch them run, or look at the objects in
the Hierarchy to see how each is put together. Copy any of them straight into
your own scene.

SparkleBurst and ImpactBurst fire once and stop. In the demo scene a small script replays them
every two seconds so the whole set stays visible. That script is demo
scenery: the prefabs do not need it, and nothing breaks if you delete Demo/.


Dropping one into your scene
----------------------------
Drag a prefab from Prefabs/ into the scene. That is the whole procedure.
The
looping effects run until the object is disabled. The one-shots play on Awake
and stop; to fire one on demand, turn off Play On Awake in the Inspector and
call Play() on the ParticleSystem.


Render pipelines
----------------
Built and verified on Unity 6, and there is nothing to switch over.

The materials use Taproot/VFX Particle, which ships in Materials/ alongside
them. It is written against UnityCG.cginc and declares no lighting pass, so
Built-in, URP and HDRP all draw it - unlike Unity's Legacy Shaders/Particles,
which do not exist outside Built-in and leave you editing every material by
hand. Drop the pack into any of the three and press Play.

The shader is worth opening if you want to change the look:

  Intensity        overall brightness, before the particle colour
  Erosion Amount   how much a particle dissolves as it dies, rather than
                   fading evenly. 0 is a plain fade
  Erosion Scale    the size of the dissolve pattern
  Tint             a colour multiplier over the whole material


Scale
-----
The effects are authored for a two metre humanoid, so each one stands at
the size it should against a character of that height. Scale the prefab's
transform to resize them. The systems simulate in local space, so the particles scale with
the transform rather than staying the size they were.


Making them your own
--------------------
Everything worth changing is on the ParticleSystem itself. Colour Over
Lifetime holds the gradient, Start Size and Start Speed set the scale of the
motion, and Emission sets the density. The materials are shared across the
pack, so tinting one tints every effect that uses it - duplicate it first if
you only want to recolour a single effect.

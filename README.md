# Knightferret RPG

My current RPG project, intendend to be well architected and flexible (unlike Caverns of Evil) -- and and also designed more as a framework, with game content 
mostly separate (as a "Test Game," not included, in the development project -- though a lot of the test game would have been the basis of a real game).  Set asside, 
at least for now, as there are other things I'd rather do with my life, and the actual game I planned to make with it was an ill-concieved whim with little direction; 
perhaps I'll go back to it later, perhaps not. I'm sure I could finish it, but again, there are other things I'd rather being doing with my life right now. 

And example of the test game as it stands using the latest version can be found here:

https://www.mediafire.com/file/lvmmkeg4x2jy6n2/RPG-WinTest.zip/file

It uses standard WASD+mouse, with E to interact and space bar to jump. 


Dev-Logs can be found here:

https://www.mediafire.com/file/lvmmkeg4x2jy6n2/RPG-WinTest.zip/file


I did consider packaging some parts, notable the Larian-esque inventory system, as an 
asset for the asset store, but it requires progamming skill to integrate into any other 
project and so does not fit the "no coding required" nature of successful code assets. 
But, rather than let it go to waste in case I don't use it myself, I'm releasing it as 
open source under a permissive license so other can use it or learn from it.

The one scene, the scene loader, will not function as is due to use of some stand-in 
assets and to referencing part of the test game as the scene to load; you would need 
provide replacements.  Also, this does require Animancer and EasySave3 to work as is; 
I'm not sure if playables would work well as an Animancer alternative, I haven't 
tried; a core feature is that items store the animations to use them, so that animation 
graphs do not need to be edited for every new items with its own animation. EasySave3 is 
used purely for serialization and other serialization solutions could probably be used 
without too many changes.

The shaders folder contains shader graphs and materials for the water shader, as well as 
several unused shaders that didn't belong anywhere else, including outline and lineart 
screen space shaders and a recreation of the spectre effect from classic doom (perhaps 
would have work for some kind of shadow people/creatures).

Many parts were based on ideas cherry picked from "Data Orient Design" as well as idea (not 
code) seen in other projects (such as having a "scene-loader" scene).  Also, static contexts 
are required to share data or move objects between scenes in Unity, for anyone wondering 
about the static "management" classes. 

This is under the MIT license. I'm open to other licensing agreements if you just don't want 
to include the license with everything (contact me if so), so long as they waive any claims 
liability or warrantee.  The main reason I use licenses instead of just dropping things as 
unlicensed freeware is to cover my own butt, just in case.

## What has happened since, and post mortem. 

After open sourcing this development was slowly continued. I cannot keep doing this.  The time 
that ends up going to this is interfering in my life in ways that are destructive to my health; 
yes, my physical health, quite substantially.  That is besides other disruptions.  And for what?  
This project is rooted in a whim, it was never a passion project nor do I expect it will make 
any money.

Pre-planning all the data structures and how they were to be saved, loaded, and handled between 
scenes would have been a good idea.  Unfortunately this was not possible because of my own 
inexperience with some of the other systems, as how the inventory system would works was also new 
and not understood in advance. 

The use of Animancer to create a framework in which animations are stored with the objects they are 
used on, but has had to difficulties as well.  Most recently, this involves animations for off-hand 
items not play.

I also never figured out how to stream scenes efficiently in order to divide the world into chunks 
or cells.  I know there is a way because several paid assets (World Streamer and RPG Creation Kit I 
know for sure) do this.  However, even with the scenes pre-loaded, simply activating and de-activing 
chunks not only failed to produce any benefit but also produced a persistent slowing of the frame 
rate (even when not near any boundaries).  

The use of static registries, as well as lists for handling things like who is healing (and idea 
borrowed directly from Data Orient Design) worked well, and though these would disturbed traditional 
OOP programmers, I find to have have worked very well and simplified many things. These were a good 
idea. 

---

## The MIT License (MIT)

### Copyright © 2025 Jared Blackburn

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
documentation files (the “Software”), to deal in the Software without restriction, including without limitation 
the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions 
of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
IN THE SOFTWARE.




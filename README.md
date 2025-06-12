# Knightferret RPG

My current RPG project, intendend to be well architected and flexible (unlike Caverns of Evil) -- and and also designed more as a framework, with game content 
mostly separate (as a "Test Game," not included, in the development project -- though a lot of the test game would have been the basis of a real game).  Set asside, 
at least for now, as there are other things I'd rather do with my life, and the actual game I planned to make with it was an ill-concieved whim with little direction; 
perhaps I'll go back to it later, perhaps not. I'm sure I could finish it, but again, there are other things I'd rather being doing with my life right now. 

And example of the test game as it stands using the latest version can be found here:

https://www.mediafire.com/file/lun1luek7pw1xoh/RPG-WinTest.2025-05-10.zip/file

It uses standard WASD+mouse, with E to interact and space bar to jump. 


Dev-Logs can be found here:

https://www.youtube.com/watch?v=agfg9jmwOBs&list=PLFJXfKtUiVHBLkaMTCRwtzBYd_ciG1XgP


I did consider packaging some parts, notable the Larian-esque inventory system, as an asset for the asset store, but it requires progamming skill to integrate into 
any other project and so does not fit the "no coding required" nature of successful code assets.  But, rather than let it go to waste in case I don't use it myself, I'm 
releasing it as open source under a permissive license so other can use it or learn from it.

The one scene, the scene loader, will not function as is due to use of some stand-in assets and to referencing part of the test game as the scene to load; you would need 
provide replacements.  Also, this does require Animancer and EasySave3 to work as is; I'm not sure if playables would work well as an Animancer alternative, I haven't 
tried; a core feature is that items store the animations to use them, so that animation graphs do not need to be edited for every new items with its own animation. 
EasySave3 is used purely for serialization and other serialization solutions could probably be used without too many changes.

The shaders folder contains shader graphs and materials for the water shader, as well as several unused shaders that didn't belong anywhere else, including outline and lineart screen space shaders and a recreation of the spectre effect from classic doom (perhaps would have work for some kind of shadow people/creatures).

Many parts were based on ideas cherry picked from "Data Orient Design" as well as idea (not code) seen in other projects (such as having a "scene-loader" scene).  Also, 
static contexts are required to share data or move objects between scenes in Unity, for anyone wondering about the static "management" classes. 

This is under the MIT license. I'm open to other licensing agreements if you just don't want to include the license with everything (contact me if so), so long as 
they waive any claims liability or warrantee.  The main reason I use licenses instead of just dropping things as unlicensed freeware is to cover my own butt, just 
in case.

The MIT License (MIT)

Copyright © 2025 Jared Blackburn

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




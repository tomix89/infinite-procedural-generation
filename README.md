I was lately amazed by:\
https://oskarstalberg.com/game/wave/wave.html \
https://github.com/mxgmn/WaveFunctionCollapse

So I decided to do my own procedural generator. This project is **NOT** using any wave function collapse!\
On the other side it **DOES** allow infinite scrolling of the generated (random) pattern with of course preserving logical continuity of tiles!\
The main reason is i just wanted a quick working test which i would like to port to esp32 + ILI9341.\
But since there is no real debug capability in arduino based esp32 projects this project was born.\
\
The main idea is that it generates only bottom to top left to right, and basically just rolls random numbers to place tiles with certain constraints.\
From all the available tiles first i filter out the ones that would fit for the next tile,\
e.g. the next tile is dependent on the previously placed one, therefore the continuity is preserved.\
\
The tiles are using a 2x2 'connection' matrix, where i tell the program which tiles are compatible with which.\
for example a tile:\
<img src="TileTest\tiles\wall_L.png"
     alt="Left Wall"
     width="100"
     style="float: left; margin-right: 10px;" />\
has a symmetry of:\
`WALL, SKY`\
`WALL, SKY`\
\
therefore on the right it can have only a tile which has compatible left side:\
`SKY, ...`\
`SKY, ...`
\
\
I also added a few higher level constraints, just because I found the result more pleasing. These are: 
1) do not put clouds below certain row number
1) do put flying ground tiles only in the middle rows 


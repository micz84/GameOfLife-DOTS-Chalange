My take on Game of Life in DOTS. I have decided to use ECS only to represent cells. The simulation uses native arrays. My simulation is without boundaries, which means cells at the left edge are neighbours of cells at the right edge, and the same for the top and the bottom. This way I avoid the if statement to check for edges. There is also a way to avoid that without this approach an example of that can be found in my other implementation of Game of Life (https://github.com/micz84/GameOfLife). I store indices of neighbours in int4x2. The state is stored as a byte, this way I can just add the value of the state to test for a number of active neighbour cells. 

*Test Machine*:

- CPU: Intel(R) Core(TM) i7-6700K CPU @ 4.00GHz   4.01 GHz
- RAM: 48,0 GB​​ 2.6 GH DDR4
- GPU: GTX 1070 8 GB

*Performance test results*:

- 85-90 FPS when simulating and displaying 512x512 cells
- 68-72 FPS when simulating 1024x1024 cells but displaying 512x512
- 20 FPS when simulating and displaying 1024x1024​
- 50 FPS when simulating  2048x2048 and displaying 8x8​
- 13 FPS when simulating  4096x4096 and displaying 8x8 - 512x5​12
- 9 FPS when simulating  4096x4096 and displaying 1024x1024​
- 4 FPS when simulating  4096x4096 and displaying 2048x2048
- 3 FPS when simulating  8192x8192 and displaying 1024x1024​

From this data, we can see that the display may be a big limiting factor for performance, but also we can see some amount of parallelism between simulation and rendering. When simulating 8192x8192 there was almost no difference between displaying 8x8 and 512x512  it took about 340 ms.

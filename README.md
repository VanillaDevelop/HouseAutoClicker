### A Lesson on FFXIV Housing

> The definition of insanity is doing the same thing over and over again and expecting a different result.  
> -- <cite>That guy from Far Cry I think</cite>

Are you familiar with the concept of Bitcoin Mining? In order to prevent forgery of a decentralized system, Bitcoin requires a "Proof of Work". Essentially, it is the equivalent to guessing a very large predetermined number first, before anyone else, in order to acquire a reward. Computers will randomly guess numbers for hours or even days on end, in hopes of hitting that one jackpot that allows them to verify a block and acquire a predetermined reward.

Housing in the hit MMORPG Final Fantasy XIV (insert the "one joke" copypasta here) functions much the same. After repeatedly stating that somehow there are not enough servers for more housing, which, considering there is only about one house for every 3 **active** players on the average FFXIV server, before you even start to consider Free Companies (Guilds), the executive producer of FFXIV has recently announced a "lottery based" housing system. 

Little does he know, we already have a lottery based housing system. When a housing plot currently becomes available, an invisible timer is started between 30 minutes and 24 hours. At any point within this timeframe, the house may become available for purchase. There is no warning or heads-up given. Whoever happens to click the plot first after it becomes available will win it. That, in essence, just makes it the dumbest version of Blockchain Mining, excluding Monkey NFTs.

> There's no mathematic advantage to spending hours clicking at a placard. The only thing that matters is who is clicking at exactly the moment that the house drops.  
> -- <cite>Some guy on the FFXIV Forums that definitely does not understand the mathematical advantage to spending hours clicking at a placard.</cite>

### The House Auto Clicker

> Its not a bot  
> Its a keystroke macro  
> Not my fault they keybound menu interactions  
> -- <cite>An ex member of the coveted Alith Free Company</cite>

Using the House Auto Clicker, you can
- Watch as your GPU fries itself as it continuously renders the same camera movement for hours on end, which is definitely not what they wanted the Amazon Rainforest to burn for.
- Play League of Legends at the same time as you read people's life stories about how many weeks they've been at this housing plot and their GF who has cancer that they want to buy the house for on your second monitor
- Click house a lot

**Features**
- Compact and easy to use, just a couple console commands.
- Controls as many clients as your PC can reasonably handle, using multi-threading 
- **Sync Mode** allows you to coordinate multiple clients to spread out their purchase attempts

### How To Use
> just get a house ![4Head](https://cdn.discordapp.com/emojis/585590830853521448.png?size=16)  
> -- <cite>Toran Kiromi from FFLogs.com</cite>

Before you start any clicker threads, make sure you have opened all XIV clients that you wish to use. Each thread directly references the Client in their order of when they were started. If you want to open another client, or close an existing one, stop all threads first (command `stop all`).
- **The code is not necessarily stable when opening additional FFXIV windows or closing existing ones in the middle of an active thread loop.**

Use the `info` command to see all your launched FF clients. Using `help`, you can see all possible commands. By default, the application uses the following 3 keybinds.
- Numpad 0: Keybind|System|Confirm
- Numpad 2: Keybind|System|Move Cursor down/Cycle down through Party List
- Numpad 6: Keybind|System|Move Cursor/Target Cursor Right

You will need to either change your ingame keybinds for these 3 options, or alter the Keybinds of the House Auto Clicker in the ConstantData.cs file and recompile the solution.

You may want to take a look at the options in `help`. Specifically, with the command `purchasefc`, you can change the key sequence to either attempt to purchase an FC house, or a personal house (default). With the `sync` command, you can enable or disable sync mode (explained below). With the `randomdelay` command, you can enable or disable a random delay after each purchase attempt, in case you think Square Enix actually cares (they don't).

You can start or stop clients with `start/stop [id]` respectively, where id is the ID of your FFXIV client in `info`. If you want to start the clicker on all clients, use `start all`. To stop all running threads, use `stop all`. For a quick check of how much time you have already wasted this session, use `sanity`. 

**What does the sync mode do?**  
If you happen to start the clicker on all clients at the same time, or if you use the random delay functionality, it may be that your clients end up temporarily or permanently syncing up their purchase attempts. You don't want this. Synchronized purchase attempts are basically just one attempt. What you want to do is minimize the maximum time between any two purchase attempts you make. Enabling sync mode lets threads start purchase attempts only after (Purchase Attempt Duration)/N milliseconds, where N is the number of currently active threads. This stops overlapping from occurring, and it's very effective. This is of course anecdotal, but immediately after I implemented sync mode, I sniped the very next available house. However, if you have accounts sitting at different houses, or are using the program for another parallel task, sync mode might slightly slow down your overall throughput.

### Contribution
Feel free to contribute via pull requests or issues. The code is provided under GPL-3.0, i.e. available without restrictions under the conditions of keeping the same license.

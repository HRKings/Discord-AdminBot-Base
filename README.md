# General Purpose Discord Admin Bot Base

This repository contains a base admin bot with extra features, it is based from [Discord.NET Bot Template](https://github.com/HRKings/DiscordNetBotTemplate) and will be updated with the latest features from the repo.

The bot feature basic admin tools, and is meant to be expanded and customized.

## Running with Docker

The project is already dockerized and can be run using the following command:

```bash
docker-compose up -d
```

The bot is setup to pick important things from environment variables:

- The token comes from `DISCORD_BOT_TOKEN` variable and needs to be changed
- The status comes from `DISCORD_BOT_ACTIVITY` variable and will default to `I'm alive!` if not changed
- The prefix comes from `DISCORD_BOT_COMMAND_PREFIX` variable and will default to `!` if not changed

I recommend looking into the dockerfile and docker-compose file, they are realy simple and can be expanded upon.

## Running without docker

If you wish to run without docker, you cna use your IDE or the command:

```bash
dotnet run
```

Just remember to set the environment variables.
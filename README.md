# About

This is a project designed to enable a way for Twitch chat to control a game of Slay the Spire. It uses ForgottenArbiter's [Communication Mod](https://github.com/ForgottenArbiter/CommunicationMod) to interact with the game, and connects to Twitch chat via IRC to consume the commands.

# How To Use

Requires .NET Core 3.1 to use. You can install this on Manjaro (and probably Arch) with:

```bash
sudo pacman -S dotnet-sdk dotnet-targeting-pack
```

For other distros you may need to refer to MS documentation.

Once installed, ensure you have Communication Mod installed and set up. Set the command in that to be `dotnet --project /path/to/this/project`

In order to connect, you'll need to create a config.json in your home directory similar to the following (with your twitch API key):

`{"Channel":"your_twitch_username","Username":"name_of_your_user_or_bot","Password":"oauth:your_oauth_token_here"}`

Then upon launching the external application in StS you should see a "Hello world!" in your Twitch chat, then things might start working.

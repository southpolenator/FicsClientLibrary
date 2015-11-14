### FicsClientLibrary

FreeInternetChessServer (freechess.org) client library is simple library for communication with FICS. Sample usage:

    FicsClient client = new FicsClient();
    await client.LoginGuest();
    var games = await client.ListGames();

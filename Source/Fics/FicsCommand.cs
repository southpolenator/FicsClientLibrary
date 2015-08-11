namespace Internet.Chess.Server.Fics
{
    //#define BLK_NULL 0
    //#define BLK_GAME_MOVE 1
    //#define BLK_ABORT 10
    //#define BLK_ACCEPT 11
    //#define BLK_ADDLIST 12
    //#define BLK_ADJOURN 13
    //#define BLK_ALLOBSERVERS 14
    //#define BLK_ASSESS 15
    //#define BLK_BACKWARD 16
    //#define BLK_BELL 17
    //#define BLK_BEST 18
    //#define BLK_BNAME 19
    //#define BLK_BOARDS 20
    //#define BLK_BSETUP 21 
    //#define BLK_BUGWHO 22
    //#define BLK_CBEST 23
    //#define BLK_CLEARMESSAGES 24
    //#define BLK_CLRSQUARE 25
    //#define BLK_CONVERT_BCF 26
    //#define BLK_CONVERT_ELO 27
    //#define BLK_CONVERT_USCF 28
    //#define BLK_COPYGAME 29
    //#define BLK_CRANK 30
    //#define BLK_CSHOUT 31
    //#define BLK_DATE 32
    //#define BLK_DECLINE 33
    //#define BLK_DRAW 34
    //#define BLK_ECO 35
    //#define BLK_EXAMINE 36
    //#define BLK_FINGER 37
    //#define BLK_FLAG 38
    //#define BLK_FLIP 39
    //#define BLK_FMESSAGE 40
    //#define BLK_FOLLOW 41
    //#define BLK_FORWARD 42
    //#define BLK_GAMES 43
    //#define BLK_GETGI 44
    //#define BLK_GETPI 45
    //#define BLK_GINFO 46
    //#define BLK_GOBOARD 47
    //#define BLK_HANDLES 48
    //#define BLK_HBEST 49
    //#define BLK_HELP 50
    //#define BLK_HISTORY 51
    //#define BLK_HRANK 52
    //#define BLK_INCHANNEL 53
    //#define BLK_INDEX 54
    //#define BLK_INFO 55
    //#define BLK_ISET 56
    //#define BLK_IT 57
    //#define BLK_IVARIABLES 58
    //#define BLK_JKILL 59
    //#define BLK_JOURNAL 60
    //#define BLK_JSAVE 61
    //#define BLK_KIBITZ 62
    //#define BLK_LIMITS 63
    //#define BLK_LINE 64 /* Not on FICS */
    //#define BLK_LLOGONS 65
    //#define BLK_LOGONS 66
    //#define BLK_MAILHELP 67
    //#define BLK_MAILMESS 68
    //#define BLK_MAILMOVES 69
    //#define BLK_MAILOLDMOVES 70
    //#define BLK_MAILSOURCE 71
    //#define BLK_MAILSTORED 72
    //#define BLK_MATCH 73
    //#define BLK_MESSAGES 74
    //#define BLK_MEXAMINE 75
    //#define BLK_MORETIME 76
    //#define BLK_MOVES 77
    //#define BLK_NEWS 78
    //#define BLK_NEXT 79
    //#define BLK_OBSERVE 80
    //#define BLK_OLDMOVES 81
    //#define BLK_OLDSTORED 82
    //#define BLK_OPEN 83
    //#define BLK_PARTNER 84
    //#define BLK_PASSWORD 85
    //#define BLK_PAUSE 86
    //#define BLK_PENDING 87
    //#define BLK_PFOLLOW 88
    //#define BLK_POBSERVE 89
    //#define BLK_PREFRESH 90
    //#define BLK_PRIMARY 91
    //#define BLK_PROMOTE 92
    //#define BLK_PSTAT 93
    //#define BLK_PTELL 94
    //#define BLK_PTIME 95
    //#define BLK_QTELL 96
    //#define BLK_QUIT 97
    //#define BLK_RANK 98
    //#define BLK_RCOPYGAME 99
    //#define BLK_RFOLLOW 100
    //#define BLK_REFRESH 101
    //#define BLK_REMATCH 102
    //#define BLK_RESIGN 103
    //#define BLK_RESUME 104
    //#define BLK_REVERT 105
    //#define BLK_ROBSERVE 106
    //#define BLK_SAY 107
    //#define BLK_SERVERS 108
    //#define BLK_SET 109
    //#define BLK_SHOUT 110
    //#define BLK_SHOWLIST 111
    //#define BLK_SIMABORT 112
    //#define BLK_SIMALLABORT 113
    //#define BLK_SIMADJOURN 114
    //#define BLK_SIMALLADJOURN 115
    //#define BLK_SIMGAMES 116
    //#define BLK_SIMMATCH 117
    //#define BLK_SIMNEXT 118
    //#define BLK_SIMOBSERVE 119
    //#define BLK_SIMOPEN 120
    //#define BLK_SIMPASS 121
    //#define BLK_SIMPREV 122
    //#define BLK_SMOVES 123
    //#define BLK_SMPOSITION 124
    //#define BLK_SPOSITION 125
    //#define BLK_STATISTICS 126
    //#define BLK_STORED 127
    //#define BLK_STYLE 128
    //#define BLK_SUBLIST 129
    //#define BLK_SWITCH 130
    //#define BLK_TAKEBACK 131
    //#define BLK_TELL 132
    //#define BLK_TIME 133
    //#define BLK_TOMOVE 134
    //#define BLK_TOURNSET 135
    //#define BLK_UNALIAS 136
    //#define BLK_UNEXAMINE 137
    //#define BLK_UNOBSERVE 138
    //#define BLK_UNPAUSE 139
    //#define BLK_UPTIME 140
    //#define BLK_USCF 141
    //#define BLK_USTAT 142
    //#define BLK_VARIABLES 143
    //#define BLK_WHENSHUT 144
    //#define BLK_WHISPER 145
    //#define BLK_WHO 146
    //#define BLK_WITHDRAW 147
    //#define BLK_WNAME 148
    //#define BLK_XKIBITZ 149
    //#define BLK_XTELL 150
    //#define BLK_XWHISPER 151
    //#define BLK_ZNOTIFY 152
    //#define BLK_REPLY 153 /* Not on FICS */
    //#define BLK_SUMMON 154
    //#define BLK_SEEK 155
    //#define BLK_UNSEEK 156
    //#define BLK_SOUGHT 157
    //#define BLK_PLAY 158
    //#define BLK_ALIAS 159
    //#define BLK_NEWBIES 160
    //#define BLK_SR 161
    //#define BLK_CA 162
    //#define BLK_TM 163
    //#define BLK_GETGAME 164
    //#define BLK_CCNEWSE 165
    //#define BLK_CCNEWSF 166
    //#define BLK_CCNEWSI 167
    //#define BLK_CCNEWSP 168
    //#define BLK_CCNEWST 169
    //#define BLK_CSNEWSE 170
    //#define BLK_CSNEWSF 171
    //#define BLK_CSNEWSI 172
    //#define BLK_CSNEWSP 173
    //#define BLK_CSNEWST 174
    //#define BLK_CTNEWSE 175
    //#define BLK_CTNEWSF 176
    //#define BLK_CTNEWSI 177
    //#define BLK_CTNEWSP 178
    //#define BLK_CTNEWST 179
    //#define BLK_CNEWS 180
    //#define BLK_SNEWS 181
    //#define BLK_TNEWS 182
    //#define BLK_RMATCH 183
    //#define BLK_RSTAT 184
    //#define BLK_CRSTAT 185
    //#define BLK_HRSTAT 186
    //#define BLK_GSTAT 187
    //#define BLK_ERROR_BADCOMMAND 512
    //#define BLK_ERROR_BADPARAMS 513
    //#define BLK_ERROR_AMBIGUOUS 514
    //#define BLK_ERROR_RIGHTS 515
    //#define BLK_ERROR_OBSOLETE 516
    //#define BLK_ERROR_REMOVED 517
    //#define BLK_ERROR_NOTPLAYING 518
    //#define BLK_ERROR_NOSEQUENCE 519
    //#define BLK_ERROR_LENGTH 520

    internal enum FicsCommand
    {
        NotSet,

        [ServerCommandName("addlist"), ServerCommandCode(12)]
        AddToList,

        [ServerCommandName("bugwho"), ServerCommandCode(22)]
        ListBughouse,

        [ServerCommandName("cshout"), ServerCommandCode(31)]
        SendChessShoutMessage,

        [ServerCommandName("follow"), ServerCommandCode(41)]
        FollowPlayer,

        [ServerCommandName("games"), ServerCommandCode(43)]
        ListGames,

        [ServerCommandName("iset"), ServerCommandCode(56)]
        SetServerInterfaceVariable,

        [ServerCommandName("ivariables"), ServerCommandCode(58)]
        ListServerInterfaceVariables,

        [ServerCommandName("moves"), ServerCommandCode(77)]
        MoveList,

        [ServerCommandName("observe"), ServerCommandCode(80)]
        ObserveGame,

        [ServerCommandName("set"), ServerCommandCode(109)]
        SetServerVariable,

        [ServerCommandName("shout"), ServerCommandCode(110)]
        SendShoutMessage,

        [ServerCommandName("showlist"), ServerCommandCode(111)]
        ShowList,

        [ServerCommandName("sublist"), ServerCommandCode(129)]
        RemoveFromList,

        [ServerCommandName("tell"), ServerCommandCode(132)]
        SendMessage,

        [ServerCommandName("unobserve"), ServerCommandCode(138)]
        UnobserveGame,

        [ServerCommandName("variables"), ServerCommandCode(143)]
        ListServerVariables,

        [ServerCommandName("who"), ServerCommandCode(146)]
        ListPlayers,
    }
}

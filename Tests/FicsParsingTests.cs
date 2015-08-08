namespace FicsClientLibraryTests
{
    using Internet.Chess.Server;
    using Internet.Chess.Server.Fics;
#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
    using System;
    using System.Diagnostics;
    using System.Linq;

    [TestClass]
    public class FicsParsingTests : TestsBase
    {
        [TestMethod, Timeout(DefaultTestTimeout)]
        public void FicsParsePartnership()
        {
            var parthership = FicsClient.ParsePartnership("1801 ^VladimirPutin / 2008 .Liverppol", true);

            Assert.IsNotNull(parthership);
            Assert.AreEqual(parthership.Player1.Rating, 1801);
            Assert.AreEqual(parthership.Player1.Username, "VladimirPutin");
            Assert.AreEqual(parthership.Player2.Rating, 2008);
            Assert.AreEqual(parthership.Player2.Username, "Liverppol");
        }

        [TestMethod, Timeout(DefaultTestTimeout)]
        public void FicsParseGameEnded()
        {
            string GameEndedMessage = "\n{Game 457 (szesze vs. aldobaral) aldobaral forfeits on time} 1-0";
            FicsClient client = new FicsClient();
            GameEndedInfo gameEndedInfo = null;

            client.GameEnded += (info) =>
            {
                gameEndedInfo = info;
            };
            client.IsKnownMessage(ref GameEndedMessage);
            Assert.IsNotNull(gameEndedInfo);
            Assert.AreEqual(gameEndedInfo.GameId, 457);
            Assert.AreEqual(gameEndedInfo.WhitePlayerUsername, "szesze");
            Assert.AreEqual(gameEndedInfo.BlackPlayerUsername, "aldobaral");
            Assert.AreEqual(gameEndedInfo.Message, "aldobaral forfeits on time");
            Assert.AreEqual(gameEndedInfo.WhitePlayerPoints, 1);
            Assert.AreEqual(gameEndedInfo.BlackPlayerPoints, 0);
        }

        [TestMethod, Timeout(DefaultTestTimeout)]
        public void FicsParseGameStoppedObserving()
        {
            string RemovingGameMessage = "\nRemoving game 129 from observation list.";
            FicsClient client = new FicsClient();
            int gameId = 0;

            client.GameStoppedObserving += (id) =>
            {
                gameId = id;
            };
            client.IsKnownMessage(ref RemovingGameMessage);
            Assert.AreEqual(gameId, 129);
        }

        [TestMethod, Timeout(DefaultTestTimeout)]
        public void FicsParseGames()
        {
            const string GamesString = @"
  1 (Setup    0 A              0 B         ) [ Bu  0   0] W:  1
  6 (Setup    0 sabretoothi    0 sabretooth) [ uu  0   0] W:  1
 17 (Exam.    0 puzzlebot      0 GuestVSHP ) [ uu  0   0] B:  1
 18 (Exam.    0 GuestGRQG      0 puzzlebot ) [ uu  0   0] W:  2
 20 (Exam.    0 puzzlebot      0 GuestXLFZ ) [ uu  0   0] B:  1
 34 (Exam.    0 GuestHGWL      0 puzzlebot ) [ uu  0   0] W: 11
 39 (Exam.    0 GuestRXYZ      0 puzzlebot ) [ uu  0   0] W:  1
 57 (Exam.    0 puzzlebot      0 GuestWJYC ) [ uu  0   0] B:  1
301 (Exam.    0 puzzlebot      0 Sedate    ) [ uu  0   0] B:  1
 10 (Exam.  100 MattWiewel   100 TimAiles  ) [ uu  0   0] W: 41
235 (Exam.    0 GuestFTSY   1197 jjamesge  ) [ bu 10   0] W: 43
248 (Exam.  803 adapala     1184 nirbak    ) [ br  5   5] B:  8
141 (Exam. 1282 AbhijitKunt 1282 AlbertSega) [ uu  0   0] W: 11
396 (Exam. 1561 trss        1427 Freerus   ) [ br  3   0] W: 26
151 (Exam. 1502 schlemazzel 1577 dragotea  ) [ br  5   0] B: 25
417 (Exam. 1605 ramparashar 1614 lamlam    ) [ br  3   0] W:  1
206 (Exam. 1694 Abasht      1674 ensaad    ) [ sr 15   0] B:  5
 73 (Exam. 2064 MrBug       2505 MiniGreat ) [ Br  2   0] W: 12
474 (Exam. 2647 GMVanWely   2383 FMTenHerto) [ su120   0] W:  1
  4 ++++ GuestHMDK   ++++ GuestNWTD  [ bu  2  12]   1:32 -  0:37 (13-14) B: 43
  7 ++++ GuestXSGJ   ++++ GuestCJHV  [ bu 10   0]   3:39 -  0:13 ( 8- 7) W: 45
  9 ++++ poopdecker  ++++ GuestBRHF  [ bu  3   0]   1:23 -  0:14 (11-17) B: 43
 11 ++++ GuestKQML   ++++ GuestBMSX  [ bu 10   3]   6:21 -  9:14 (19-26) W: 15
 15 ++++ GuestHJTL   ++++ getga      [ bu  3   0]   2:39 -  2:13 (36-36) B: 13
 22 ++++ GuestDFXC   ++++ GuestTKSY  [ bu  2  12]   3:51 -  3:53 (38-35) W: 14
 25 ++++ GuestPJQG   ++++ GuestBTGK  [ bu 10   0]   9:38 -  9:55 (34-39) W:  3
 27 ++++ GuestWZDH   ++++ GuestYZHJ  [ bu  5   0]   4:40 -  4:18 (38-36) W: 10
 31 ++++ GuestMPHR   ++++ GuestDZSZ  [ lu  1   0]   0:00 -  0:09 (30-30) W: 23
 35 ++++ GuestCHNL   ++++ GuestSCQB  [ su 15   0]  10:20 - 12:55 (23-23) B: 21
 36 ++++ GuestCBBF   ++++ GuestFSHC  [ bu  3   5]   3:02 -  3:02 (39-39) W:  3
 37 ++++ GuestLBPX   ++++ azigzagz   [ bu 10   5]   7:58 -  8:48 (39-39) W: 10
 43 ++++ GuestQBBH   ++++ GuestKMVQ  [ su 15   0]  10:05 - 11:45 (30-32) W: 20
 44 ++++ GuestZXPD   ++++ GuestDVFM  [ bu  5   5]   5:01 -  5:23 (18-19) W: 22
 45 ++++ GuestXQQW   ++++ GuestLKJB  [ bu  5   5]   5:22 -  1:39 (33-30) W: 21
 49 ++++ playtoe     ++++ GuestVCRN  [ su 18  30]  20:14 - 18:37 (38-37) B: 11
 53 ++++ GuestBTGM   ++++ GuestVQCM  [ bu  3   0]   1:24 -  1:04 (16-17) B: 38
 54 ++++ GuestDYKT   ++++ dawizza    [ bu  5   0]   4:26 -  4:54 (38-39) W:  7
 59 ++++ GuestWGBR   ++++ GuestNKMP  [ su 15   0]  12:50 - 12:12 (30-29) B: 19
 67 ++++ GuestZCWX   ++++ GuestVYNJ  [ bu  2  12]   3:13 -  3:53 (19-23) B: 22
 71 ++++ GuestTVMV   ++++ GuestKGGN  [ su 15   5]  13:41 - 12:52 (38-38) W: 17
 72 ++++ GuestHLKI   ++++ GuestGGPW  [ su 15   0]  12:40 - 13:09 (31-34) W: 18
 74 ++++ kicked      ++++ GuestBTVG  [ bu 10   0]   7:10 -  7:01 (34-31) B: 17
 75 ++++ GuestWZXY   ++++ GuestHFJG  [ bu 10   0]   5:06 -  7:58 (31-33) W: 17
 82 ++++ GuestPHTM   ++++ GuestCTGP  [ bu  3   5]   3:28 -  2:49 (23-22) B: 23
 91 ++++ GuestCQJV   ++++ GuestHWHL  [ bu 10   0]   6:05 -  5:09 (29-30) W: 23
 92 ++++ GuestVXKX   ++++ GuestXDZH  [ bu  5   0]   4:49 -  4:50 (39-39) B:  5
 99 ++++ GuestHNYN   ++++ guesth     [ su 15   5]   9:52 - 13:40 (30-39) W: 11
100 ++++ GuestQXZV   ++++ GuestLBWR  [ bu  5   0]   3:04 -  2:43 (17-15) B: 32
101 ++++ GuestDEDL   ++++ GuestBRDZ  [ bu 10   5]  10:03 -  8:54 (39-39) B:  4
102 ++++ GuestXPRF   ++++ GuestBDFC  [ su 25   0]  24:54 - 24:56 (39-39) B:  3
106 ++++ AmericanBot ++++ gfgjgh     [ bu  5   0]   4:29 -  4:30 (39-39) B: 11
117 ++++ GuestDQWC   ++++ Doggydee   [ bu  5   0]   3:32 -  3:55 (36-36) W: 13
118 ++++ GuestKKVC   ++++ GuestGQBM  [ su 15   3]  14:47 - 14:43 (35-38) W:  8
119 ++++ GuestHVNL   ++++ marlenaros [ su 15   0]  13:38 - 13:40 (38-37) W: 13
120 ++++ GuestRZJF   ++++ GuestDYRR  [ bu  3   5]   3:17 -  2:08 (14-11) B: 32
121 ++++ GuestGTFT   ++++ GuestTYPV  [ su 15   0]  13:06 - 13:34 (35-35) B: 12
123 ++++ GuestQBRY   ++++ GuestNCHM  [ bu 10   0]   7:52 -  6:26 (18-18) W: 26
125 ++++ GuestJDDR   ++++ GuestZZCB  [ bu  1   5]   1:17 -  0:27 (30-36) B: 22
128 ++++ GuestLoki   ++++ GusetYHHN  [ bu 10   0]   9:28 -  9:51 (39-39) B:  5
129 ++++ GuestDVKD   ++++ GuestLGFT  [ bu 10   0]   9:11 -  8:12 (35-35) B: 15
131 ++++ GuestZPGQ   ++++ GuestFHYT  [ bu 10   0]   5:19 -  5:09 (19-19) B: 27
132 ++++ GuestZLPV   ++++ GuestPFST  [ su 15   0]  14:15 - 14:09 (38-38) W:  8
133 ++++ GuestGDRC   ++++ GuestXDSV  [ bu 10   0]   6:21 -  4:15 (38-31) W: 15
135 ++++ GuestHZTG   ++++ loupgris   [ su 20   0]  19:06 - 19:04 (37-37) W:  9
138 ++++ GuestGJKT   ++++ GuestGGRX  [ bu 10   0]   8:31 -  7:03 (35-33) B: 13
139 ++++ GuestQTPW   ++++ GuestXXGZ  [ bu 10   0]   8:33 -  8:07 (30-30) B: 17
140 ++++ GuestMQVV   ++++ GZKT       [ su 40   0]  33:02 - 33:42 (31-32) W: 14
142 ++++ GuestQBJT   ++++ GuestRHRJ  [ bu  5   1]   3:26 -  3:23 (32-31) W: 17
148 ++++ GuestKTMC   ++++ GuestNNRJ  [ lu  1   0]   1:00 -  1:00 (39-39) W:  1
149 ++++ GuestPLFG   ++++ DWDD       [ bu  3   0]   1:09 -  1:27 ( 9- 9) W: 31
155 ++++ GuestFTFR   ++++ GuestMFQT  [ bu  5   5]   0:52 -  2:23 (23-27) W: 20
162 ++++ GuestNMBS   ++++ GuestDJHK  [ bu  5   0]   2:21 -  4:04 (27-31) W: 22
163 ++++ GuestZBZS   ++++ GuestNMDB  [ bu  5   0]   3:25 -  3:30 (35-35) W: 21
166 ++++ GuestSZVJ   ++++ LeeRoyalz  [ su 15   5]  13:17 - 14:45 (26-25) W: 25
167 ++++ GuestYKTW   ++++ GuestSVFS  [ bu  3   0]   0:07 -  0:50 (20-18) W: 35
177 ++++ GuestPZQC   ++++ GuestGFJY  [ bu  3   0]   1:19 -  1:35 (24-24) W: 20
180 ++++ GuestJMFH   ++++ GuestMDHJ  [ bu 10   0]   7:14 -  7:45 (37-36) B: 15
181 ++++ GuestKNCT   ++++ GuestNZTZ  [ bu 10   5]   9:56 -  9:43 (35-33) B: 16
187 ++++ hulkzh      ++++ GuestXKYF  [ bu  2  15]   2:00 -  2:00 (39-39) W:  1
188 ++++ iplayfaster ++++ GuestGORO  [ bu  3   0]   1:33 -  1:11 (23-13) B: 28
190 ++++ GuestDNFD   ++++ GuestZKNV  [ bu 10   0]   6:27 -  7:24 (38-38) W: 16
193 ++++ Blucreyalp  ++++ japinero   [ bu  8   0]   4:38 -  5:53 (35-34) B: 19
194 ++++ uhgyff      ++++ GuestPXYP  [ bu  5   0]   3:57 -  4:35 (35-35) B: 11
196 ++++ xxxxxxx     ++++ GuestRYJF  [ bu 10   5]   4:21 -  5:34 (33-34) B: 23
197 ++++ GuestBSSB   ++++ GuestDDBl  [ bu  5   0]   4:47 -  4:13 (34-35) W:  8
200 ++++ GuestFPJQ   ++++ GuestTJSG  [ bu 10   0]   8:30 -  9:26 (37-38) W:  8
201 ++++ GuestXPQM   ++++ GuestWGKX  [ bu 10   0]   7:26 -  8:27 (28-27) W: 21
211 ++++ asadhavej   ++++ guestMWMW  [ bu 10   2]   5:47 -  7:03 (19-18) W: 24
213 ++++ GuestJKXY   ++++ GuestRFRT  [ bu  5  10]   3:41 -  3:18 (22-27) W: 22
214 ++++ GuestGQVM   ++++ GuestLMHY  [ bu 10   0]   6:33 -  6:29 (18-18) W: 32
215 ++++ wwerig      ++++ GuestJKVF  [ su 15   0]   8:01 - 11:24 (35-34) B: 13
217 ++++ GuestFCPY   ++++ GuestXQJL  [ bu 10   0]   5:18 -  4:49 (26-16) W: 24
219 ---- pakaya      ++++ GuestHHTY  [ su 15   0]  14:54 - 14:54 (39-39) W:  4
225 ++++ GuestFWRR   ++++ GuestRZTM  [ su 30   0]  26:06 - 21:21 (29-31) W: 21
226 ++++ GuestSRHV   ++++ GuestWWJV  [ su 15   0]  12:54 - 10:36 (31-30) W: 16
227 ++++ GuestRNXX   ++++ GuestBJCC  [ bu 10   0]   8:47 -  9:41 (39-39) B:  7
229 ++++ GuestHNBT   ++++ GuestVSRB  [ su 20   0]  16:39 - 18:13 (29-29) W: 22
230 ++++ GuestHBGH   ++++ GuestCQTZ  [ bu 10   0]   4:54 -  0:36 (19-17) B: 23
231 ++++ GuestGPNP   ++++ GuestVLTR  [ bu  5   0]   4:19 -  4:11 (36-36) B: 11
233 ++++ GuestDMXF   ++++ GuestBTLD  [ bu  5   0]   3:57 -  3:58 (28-29) B: 13
234 ++++ GuestMTNQ   ++++ GuestLSCQ  [ bu 10   0]   6:45 -  7:00 (26-26) W: 21
237 ++++ GuestLJMN   ++++ GuestXQBT  [ bu  5   0]   3:47 -  2:51 (32-30) W: 18
239 ++++ GuestTDWD   ++++ GuestVNZR  [ bu 10   0]  10:00 - 10:00 (39-39) W:  1
240 ++++ GuestHTFS   ++++ GuestYNGP  [ su 15   0]  10:22 - 12:26 (26-26) B: 26
246 ++++ GuestNGJV   ++++ Moeletsi   [ su 30   5]  21:20 - 28:34 (31-25) B: 29
249 ++++ GuestGQSL   ++++ GuestZDVK  [ bu 12   0]   9:10 -  8:46 (26-27) B: 22
255 ++++ GuestRTPP   ++++ GuestNKTV  [ bu 10   0]  10:00 - 10:00 (39-39) B:  1
259 ++++ lkoiujh     ++++ wortger    [ bu 10   0]   6:36 -  5:42 (26-26) B: 24
260 ++++ GuestZYKY   ++++ GuestGJMB  [ su 15   0]  11:09 - 13:43 (30-35) B: 15
261 ++++ SooYouLose  ++++ GuestSLXS  [ su 15   0]   9:36 -  7:33 (33-32) B: 28
263 ++++ GuestKNLH   ++++ GuestXPKZ  [ bu  5   5]   4:56 -  5:03 (39-39) W:  3
264 ++++ GuestBTNG   ++++ GuestSXZC  [ su 15   0]  12:19 - 11:16 (35-35) W: 17
267 ++++ GuestCLQL   ++++ GuestVFHD  [ bu 10   0]   8:50 -  8:46 (38-39) W: 15
269 ++++ GuestDTPY   ++++ GuestYPPG  [ bu  5   0]   0:13 -  1:01 ( 7-13) B: 52
271 ++++ GuestQBFF   ++++ GuestXBTL  [ bu  5   0]   3:19 -  3:57 (21-25) B: 23
273 ++++ GuestLQHQ   ++++ thebestand [ su 15   5]  10:15 -  1:03 (31-32) W: 23
274 ++++ GuestSCZR   ++++ GuestDVVG  [ bu  4   0]   2:28 -  2:54 (31-31) B: 13
278 ++++ GuestFHYF   ++++ GuestLQVC  [ bu 10   0]   7:53 -  8:23 (19-17) W: 28
279 ++++ GuestJVYT   ++++ GuestKVQN  [ bu 10   5]   7:50 -  9:50 (29-29) B: 23
281 ++++ GuestPVVX   ---- kinghm     [ su 30   0]  26:36 - 24:11 (39-39) W:  7
286 ++++ GuestHWWH   ++++ GuestMRWX  [ su 15   0]   9:31 - 11:33 (21-33) W: 29
291 ++++ guestTDMRG  ++++ GuestWPTV  [ lu  2   0]   0:33 -  0:18 (35-35) B: 26
292 ++++ GuestYVQN   ++++ GuestBTFS  [ bu 10   0]   6:50 -  8:52 (34-33) B: 15
293 ++++ GuestLYZP   ++++ GuestZRMW  [ su 15   0]   9:23 - 10:20 (27-35) W: 21
298 ++++ GuestXQCF   ++++ GuestYQDW  [ bu 10   5]   9:31 -  6:38 (33-34) B: 13
302 ++++ GuestFLYR   ++++ GuestKRRP  [ bu  3   0]   2:02 -  1:18 (29-27) B: 21
305 ++++ GuestWCNK   ++++ GuestBMQP  [ bu 10   0]   7:54 -  8:43 (34-35) W: 15
306 ++++ peyeyo      ++++ GuestRCBN  [ su 15   5]  12:53 -  4:43 (36-36) B: 11
312 ++++ GuestAB     ++++ GuestPSGL  [ bu  5   0]   4:52 -  4:53 (39-39) W:  6
313 ++++ GuestHVRS   ++++ GuestPFVB  [ bu 10   0]   8:33 -  3:36 (10-14) B: 31
322 ++++ GuestXMBR   ++++ GuestHLLF  [ bu  5   1]   2:04 -  1:35 (27-22) B: 31
323 ++++ GuestTHZT   ++++ GuestTQSD  [ bu  4   0]   3:31 -  3:15 (33-32) W: 13
324 ++++ GuestMQWC   ++++ GuestTYYD  [ bu 10   5]   6:45 -  5:06 (35-38) W: 12
341 ++++ GuestFZZZ   ++++ GuestRJBD  [ bu  0   5]   0:09 -  0:39 (38-37) B: 22
343 ++++ GuestLBNP   ++++ GuestHHBT  [ bu  5   0]   4:37 -  4:30 (36-35) B:  9
344 ++++ GuestRZLP   ++++ GuestDHPR  [ bu  3   0]   1:38 -  1:26 (17-14) W: 28
346 ++++ GuestLDJD   ++++ GuestKDBN  [ bu  5   0]   1:28 -  2:29 (23-23) B: 15
350 ++++ GuestHLTQ   ++++ GuestBBSX  [ bu 10   0]   5:21 -  1:18 (26-18) B: 24
352 ++++ GuestNHND   ++++ GuestLFYW  [ bu  5   0]   4:19 -  3:55 (32-33) B: 16
356 ++++ GuestVYJV   ++++ WakeUpNugg [ su 15   0]  14:36 - 14:43 (39-39) B:  4
359 ++++ GuestXBVG   ++++ Shieldmaid [ bu 10   0]   9:40 -  9:52 (38-38) W:  6
367 ++++ GuestTZHB   ++++ GuestRZSL  [ bu  5   0]   5:00 -  5:00 (39-39) W:  1
369 ++++ GuestWCTF   ++++ rbpl       [ bu  1   5]   1:17 -  1:20 (27-27) B: 21
379 ++++ GuestTHPW   ++++ GuestTBNM  [ bu 10   0]   8:23 -  7:26 (29-30) B: 22
382 ++++ GuestXHXZ   ++++ GuestJMXT  [ su 15   0]  12:55 - 13:23 (35-35) B: 13
383 ++++ ewat        ++++ GuestMPMM  [ lu  2   0]   1:54 -  1:49 (34-34) W:  9
387 ++++ GuestZCWZ   ++++ GuestCDZX  [ su  5  15]   5:38 -  5:06 (39-39) B:  4
389 ++++ GuestKGPC   ++++ GuestPWNJ  [ bu  5   5]   4:10 -  5:03 (29-28) B: 19
391 ++++ GuestKAQW   ++++ GuestLTWL  [ bu 10   5]  10:00 - 10:00 (39-39) W:  1
394 ++++ GuestYXPL   ++++ GuestXCTB  [ su 15   5]  14:02 - 14:48 (35-38) B: 10
399 ++++ GuestLPLT   ++++ GuestYBCW  [ bu 10   5]   2:40 -  5:07 ( 7- 7) W: 44
402 ++++ GuestPJLR   ++++ nocego     [ bu  5   0]   1:06 -  2:11 (17-17) W: 30
404 ++++ GuestVXRB   ++++ GuestRLTJ  [ bu 10   0]   4:48 -  4:44 (22-22) B: 30
409 ++++ GuestFCNK   ++++ GuestSQLH  [ su 15   0]  13:18 - 13:11 (32-36) B: 14
411 ++++ GuestYWYF   ++++ GuestZDHQ  [ su 90  45] 1:37:02 -1:37:24 (35-29) B: 14
412 ++++ GuestRHZL   ++++ GuestMFBL  [ su 15   0]  11:03 - 10:52 (35-35) W: 19
413 ++++ GuestJFBD   ++++ GuestDWSM  [ su 25  10]  26:25 - 25:39 (35-35) B: 16
421 ++++ cachou      ++++ GuestRPRC  [ bu 10   0]   6:30 -  5:37 (30-22) B: 28
423 ++++ GuestZWCD   ++++ GuestCHGD  [ bu  5   5]   5:02 -  4:33 (33-32) W: 16
426 ++++ guestjag    ++++ gemimo     [ bu  5   5]   2:19 -  0:14 (24-21) W: 29
427 ++++ GuestPPFK   ++++ GuestLDYG  [ bu 10   0]   9:08 -  9:21 (36-37) W: 11
428 ++++ GuestKVWT   ++++ vilzrz     [ bu  5   0]   4:27 -  4:18 (33-34) W: 11
429 ++++ GuestBRCV   ++++ GuestGRYG  [ su 30   5]  23:28 - 22:25 (23-29) W: 26
442 ++++ GuestTVWN   ++++ GuestDTSV  [ lu  2   0]   1:43 -  1:31 (33-32) B: 11
445 ++++ GuestXSLD   ++++ GuestXNVP  [ bu 10   0]   8:44 -  8:31 (34-32) B: 16
447 ++++ GuestGFLQ   ++++ wswsw      [ bu 10   5]  10:07 - 10:06 (38-39) W:  4
451 ++++ GuestPLMH   ++++ GuestRHGJ  [ bu  5   0]   3:40 -  4:11 (32-32) B: 17
453 ++++ aaaaahahaha ++++ GuestNBKS  [ bu 10   0]   9:25 -  8:57 (34-35) B: 11
455 ++++ Guestboyzz  ++++ GuestRGHP  [ bu  2   2]   1:40 -  1:43 (37-38) W: 10
373  817 bnmrpsc     ++++ GuestGRCV  [ bu  5   5]   5:00 -  5:00 (39-39) B:  1
371 ++++ GuestTTSP    934 juventinoc [ bu  5   2]   3:39 -  3:19 (27-20) B: 18
 16 1033 deltavec    ++++ GuestJGHF  [ bu 10   0]   9:18 -  9:47 (39-39) W:  5
124 1083 olaflena    ++++ GuestWXCQ  [ bu  2  12]   2:57 -  3:07 (30-32) B: 15
 95 ++++ GuestQXDV   1140 yacass     [ bu  5   0]   4:38 -  4:53 (39-38) B:  4
311 ++++ GuestTWNX   1154 voldemorte [ bu  5   2]   4:44 -  4:58 (39-39) W:  3
145 ++++ GuestSVQL   1190 foxone     [ bu  3   0]   2:20 -  1:26 (28-27) B: 13
265 1201 bishopsandr ---- DaryaPg    [ bu  5  14]   5:33 -  4:32 (38-37) B:  6
349 ++++ GuestWDND   1233 Akmoses    [ bu  5   5]   4:41 -  0:33 (24-28) B: 34
 13 1266 cnellgreen  ++++ Juliogf    [ su 15   0]   2:28 -  9:48 (20-20) W: 41
 83 ++++ GuestDHHS   1271 mucchio    [ bu 10   0]   7:49 -  8:14 (31-29) W: 17
450 ++++ GuestXVYCD  1308 josejoaf   [ bu 10   0]   8:51 -  8:56 (32-35) W: 15
385 ++++ GuestTCBG   1333 caratheodo [ su 15   0]  10:00 - 11:40 (34-38) B: 12
 38 1349 gediminasb  ++++ GuestCBNS  [ bu 10   5]   7:01 -  9:20 (31-30) W: 28
153 ++++ GuestHVTJ   1401 BrownQ     [ bu  5   0]   2:11 -  2:43 (28-28) W: 22
334 ++++ GuestPZGC   1409 EddieTheOt [ bu  3  15]   4:41 -  5:05 (23-23) W: 15
222 1410 cfmr        ++++ GuestGKCD  [ su 30   5]  30:00 - 29:59 (39-39) B:  4
156 1484 Patzertroll ++++ GuestLRRT  [ bu  3   5]   2:30 -  3:30 (22-21) W: 26
325 1518 imreoir     ++++ GuestQRNK  [ bu  5   5]   5:03 -  4:53 (39-39) B:  2
318 1548 qqqzzz      ++++ juangc     [ bu 10   0]   5:43 -  2:56 (32-28) B: 30
 68 ---- Mojjan      1638 GemarMainC [ zu  5   0]   3:19 -  2:36 (37-41) B: 18
401 1701 adlkaslad   ++++ GuestZGFW  [ bu  3   0]   2:55 -  2:56 (38-39) W:  4
127 ++++ GuestYLYT   1740 chicapucp  [ bu  3   0]   1:48 -  2:02 (18-17) B: 28
319 1752 marquisce   ++++ GuestMDTG  [ bu  2  12]   3:20 -  0:38 (36-35) B: 11
270 ++++ FutureFord  1795 eroscris   [ su 30   0]  24:16 - 13:03 (34-34) B: 19
 60  784 adapala     1027 moraes     [ br  5   5]   0:49 -  2:27 (32-35) W: 18
335 1904 Poggos      ++++ GuestVBHM  [ bu  3   2]   2:19 -  1:49 (32-32) W: 18
376  995 yorikfrance  916 turnipforw [ br  2  12]   2:18 -  2:22 (39-39) W:  4
348 1921 jgkone      ++++ opmentor   [ lu  0   1]   0:11 -  0:13 (38-38) W:  7
384  963 Kaltchiley   972 suejane    [pbr  5  12]   5:03 -  5:17 (28-26) B: 19
224  924 novadim     1043 orwellvisi [ br  2  12]   2:51 -  2:47 (39-39) B:  9
171 1019 roberttdani 1028 Haoba      [ br 10   0]   8:05 -  8:56 (32-31) W: 15
315  947 DRDNYANESH  1124 neni       [ br  2  12]   3:40 -  3:36 (21-33) W: 17
361  942 andyboot    1156 OlegMeiste [ br 10   0]   9:36 -  9:35 (39-39) B:  6
136 1036 SkorpionEST 1080 radwolf    [ br  7   5]   6:57 -  6:24 (35-35) B: 12
210  985 aaquib      1163 grivnash   [ br 10   5]   9:25 -  9:17 (22-21) W: 16
 64 1184 ivanojeda   1000 yoyokhan   [ br 10   0]   5:05 -  7:25 (28-31) W: 21
347 1102 vitperov    1091 twoods     [pbr  4   0]   2:25 -  1:28 (28-31) B: 22
150 1119 frolova     1098 Cibola     [ br  2  12]   7:03 -  4:22 (14-16) B: 40
105 1209 atesh       1022 shaanpatha [ br 10   5]   8:28 -  6:32 (32-31) W: 18
303 1185 Amthatgae   1126 Ooty       [ br  5   5]   5:02 -  5:09 (36-36) W:  6
 48 1013 Hobit       1306 knighttree [ br 10   0]   7:14 -  6:30 (22-29) B: 23
258 1227 youssefhj   1092 Dovolenka  [ br 10   5]   6:19 -  6:09 (28-28) B: 24
137 1207 turmsegler  1118 laserknigh [ br  5   0]   4:15 -  4:09 (38-38) B: 10
283 1234 Vitk        1093 BenutzerCo [ br 10   0]   5:59 -  5:58 (11- 0) W: 67
103 1352 hotelrohini  985 martynssky [ br  5   5]   4:39 -  3:43 (39-38) B:  7
199 1120 mafimoya    1227 VortexD    [ br  2  12]   5:06 -  3:52 (27-27) W: 22
 97 1200 KaaleppiTuh 1194 RafaelMGS  [ br 10   0]   8:52 -  8:32 (35-35) B: 13
183 1271 febrsch     1126 paolinosco [ br  2  12]   1:38 -  2:16 (34-32) B: 15
338 1234 lilibah     1164 drtim      [ br 10   0]   6:27 -  6:20 (35-35) B: 23
165 1141 WhatKnight  1262 ausiuguk   [ br  5   0]   4:46 -  4:44 (38-39) W:  8
414 1210 iamredeemed 1207 HGBoone    [ br  2   8]   2:21 -  1:56 (37-38) W: 12
107 1149 ratherBmati 1273 couto      [ br  5   0]   3:53 -  4:18 (35-35) B: 11
 96 1222 adolbo      1201 Suita      [ br 10   0]   9:52 -  9:04 (39-39) B:  4
 81 1168 Maal        1281 DigitalSer [ br  3  10]   2:37 -  1:39 (26-25) B: 13
435 1371 Aeetes      1080 CHESSUR    [ br  5   0]   3:05 -  3:21 (13-15) W: 26
216 1213 hilterchan  1242 cynero     [ br  5   0]   4:25 -  4:34 (35-35) B:  7
406 1374 Hvergang    1082 RulEriv    [ bu  2  12]   2:54 -  1:42 (14-11) B: 22
242  963 Claudiuturc 1501 callipygia [ br  5   0]   4:49 -  5:00 (34-35) W:  7
160 1322 djgiftgas   1153 danieljime [ br  5   0]   1:44 -  0:17 ( 5- 6) B: 49
364 1348 bingobongo  1139 vastlov    [ br  3   0]   2:55 -  2:58 (39-36) B:  3
220 1138 manureva    1358 docmax     [ br  3   2]   1:16 -  2:20 (31-31) W: 20
441 1209 roncyg      1295 criololoco [ br  3  12]   0:25 -  3:37 (27-29) W: 26
209 1401 StrikeEagle 1137 hundredmov [ br  5   0]   4:10 -  3:54 (27-27) B: 13
330 1335 texasweb    1204 gbrisset   [ br  5   0]   3:39 -  2:09 (28-27) W: 22
415 1273 Obbo        1272 Sina       [ br  2  12]   0:46 -  2:43 (14-18) W: 31
182 1264 HollandMove 1293 TheLock    [pbr  5   5]   4:27 -  3:01 (19-27) B: 21
143 1001 emries      1564 SacriFish  [ br  5   3]   4:01 -  4:53 (38-38) B:  9
238 1402 Barysh      1172 MrScrumps  [ br  2  12]   2:42 -  4:02 (28-25) B: 27
176 1241 rkmallavara 1337 riera      [ br  3   0]   2:49 -  2:50 (35-36) W:  7
314 1273 afeemomo    1308 shepherd   [ br  5   0]   1:47 -  1:59 (21-22) W: 24
 26 1254 oubok       1332 wouayo     [ br 10   4]   8:22 -  6:24 (33-34) W: 16
251 1358 GMCc        1233 Milanmilli [ br  5   0]   4:46 -  4:52 (39-39) W:  5
218 1324 reydevagos  1272 flooppie   [ br  5   3]   1:16 -  4:07 (32-33) W: 25
419 1340 Zydderf     1257 ahmedbh    [ br 10   0]   7:00 -  7:10 (19-16) W: 32
144 1290 maxsolveig  1321 ecirp      [ br  5   2]   3:25 -  4:06 (34-34) W: 22
 79 1373 sbsbsb      1241 mlion      [ br  5   0]   4:17 -  4:20 (38-38) B: 12
300 1330 Moohoohaaha 1284 cabrainvoc [ br  3   1]   2:30 -  2:19 (28-32) W: 17
374 1355 dimebt      1263 Bachisss   [ sr 15   0]  12:02 -  6:09 (31-32) B: 21
365 1242 soopaman    1383 TonhoLetIt [ br  3   0]   2:20 -  2:17 (29-26) B: 15
191 1346 catriac     1281 Deseo      [ br  5   0]   3:30 -  4:04 (35-35) W: 13
245 1298 DropClose   1330 FredNerk   [ br  3   0]   1:11 -  0:28 (25-23) B: 25
 85 1334 elgrande    1295 NaturalPaw [ br 10   0]   6:04 -  2:23 (24-26) B: 23
110 1335 bishopseven 1294 wdywdpushe [ br  3   0]   2:22 -  1:56 (34-34) W: 18
159 1285 NoEuwe      1361 arunj      [pbr  5  12]   8:11 -  6:21 (13-12) B: 41
375 1330 DrYurich    1324 lorenzoBBB [ br  5   0]   4:19 -  4:29 (38-37) W: 10
439 1349 manueljaca  1312 robertSchu [ br  5   0]   2:28 -  1:57 (28-28) W: 31
 52 1278 knanah      1384 KevinSI    [ sr 15   0]  11:18 -  9:40 (25-19) B: 24
113 1262 inemuri     1402 imanon     [ br  5   0]   4:57 -  3:54 (29-22) B: 24
 94 1341 Wicks       1339 drpesic    [ br  5   0]   3:39 -  2:50 (34-34) B: 20
288 1365 biotdi      1319 sanniquell [ br  5   0]   4:31 -  4:21 (38-38) B: 11
339 1135 lllllllllll 1550 HunGuard   [ br  5  12]   1:07 -  4:03 (36-36) W: 16
256 1314 iszalados   1379 Elfuerte   [ sr 30   0]  13:14 - 25:31 (33-19) W: 22
277 1354 cutika      1348 Klonkku    [ br  5   0]   4:08 -  4:36 (39-39) B: 11
 77 1124 fujdur      1586 WelchSprin [ bu  3   0]   2:14 -  2:27 (39-39) B: 12
161 1404 Dancig      1307 anantkum   [ br  5   0]   3:36 -  3:19 (16-13) W: 28
146 1366 chirva      1348 ursuscvx   [ br 10   7]   3:37 - 10:29 (31-31) W: 25
 55 1286 JuanGustavo 1430 khans      [ br  4  15]   2:43 -  1:00 (23-23) W: 20
257 1266 Porphyrius  1454 josdwyer   [ sr 15   0]  15:00 - 15:00 (39-39) W:  1
184 1435 defensamaya 1288 jesest     [ br  3   0]   2:23 -  2:21 (38-37) B: 19
331 1405 Afafa       1322 BabyLurKin [ lr  2   0]   1:32 -  1:33 (34-35) W: 12
354 1378 Pingkan     1353 synews     [ br  5   0]   3:53 -  4:10 (32-31) W: 14
130 1399 capricornus 1335 lancent    [ br  5   0]   0:54 -  0:28 (11-10) W: 48
437 1385 sarosha     1349 SaturnRise [ br  6   0]   4:23 -  4:08 (33-35) W: 18
329 1374 boinaverde  1363 Jusis      [ br  3   0]   2:00 -  2:01 (27-24) B: 28
 93 1420 routaran    1318 TundraWalk [ br  2  12]   2:31 -  2:31 (39-39) W:  5
436 1358 DLeviathan  1380 JaBurdon   [ br  5   0]   2:53 -  3:51 (31-31) W: 20
 40 1344 Gsgsgs      1403 janjansen  [ br  3   0]   1:07 -  0:52 (20-18) B: 32
372 1400 Fleskusflin 1349 Mucke      [ br  3   0]   2:38 -  2:21 (38-32) B: 10
386 1383 dwillnu     1373 perepoma   [ br  5   0]   3:30 -  3:02 (21-21) B: 25
253 1429 korchoi     1330 willyaplay [ br  5   0]   3:03 -  4:02 (27-30) B: 19
154 1381 iwishiwasta 1380 JugadorVal [ br  5   0]   3:18 -  3:01 (28-28) W: 20
 63 1333 Rittervonun 1434 Iffieman   [ br  5   0]   2:50 -  2:59 (11-12) W: 34
186 1454 mandyr      1317 Sasosgf    [ lr  2   1]   1:53 -  1:57 (28-22) B: 10
320 1403 patriotscou 1386 thowund    [ br  3   0]   2:13 -  2:21 (33-32) B: 19
207 1475 sanslor     1317 Kyobutungi [psr 15   0]  11:49 - 13:06 (18-22) W: 29
228 1291 kitalacio   1515 DrUKahn    [ br  5   0]   0:49 -  2:15 (12- 1) B: 43
275 1486 Zrig        1321 Codjigger  [ br  2  12]   3:27 -  7:15 ( 6- 8) W: 67
168 1437 marioleston 1373 juliocebal [ lr  1   0]   0:49 -  0:33 (35-35) W: 15
351 1522 Osmed       1289 Alientoech [ br  5   0]   2:12 -  2:56 (34-33) W: 24
 29 1404 Jardass     1414 brickcityn [ sr 30   5]  29:07 - 27:08 (37-36) B: 15
  3 1358 dlogan      1461 gisab      [ br  4   0]   1:16 -  1:55 (20-20) B: 23
284 1369 coolreit    1452 philmstar  [ br  2   4]   2:22 -  2:21 (26-26) B: 31
 28 1424 ndachess    1404 gelgel     [ br  5   0]   4:19 -  4:31 (35-32) B: 10
310 1395 brogs       1433 OrianasDad [ br  5   5]   3:20 -  2:31 (19-19) W: 24
395 1524 amitietk    1309 forqueray  [ br  3   0]   1:14 -  0:02 (16-13) B: 35
114 1314 Turamon     1535 PastorK    [ br  3   0]   1:31 -  2:06 (28-28) B: 18
 14 1502 novix       1349 flavo      [ br  3   0]   2:52 -  2:57 (38-38) B:  6
170 1379 sergioluizb 1476 flyingpawn [ br  3   0]   1:23 -  1:30 (15-15) W: 31
104 1398 GranChacho  1461 TPGYRENE   [ br  5   0]   4:21 -  3:23 (34-33) B: 17
 21 1455 lionjoshi   1407 ektwo      [ sr 15   5]  14:32 - 10:18 (32-29) W: 16
 98 1565 jparra      1300 ejy        [ br  6   0]   1:30 -  1:47 ( 9- 8) B: 40
308 1416 aaronjseatt 1449 thezli     [pbr  3   0]   2:26 -  2:22 (31-31) W: 20
422 1405 Carcinogen  1473 sunnysoul  [ sr 15   0]  13:55 - 13:01 (38-39) B:  8
 46 1430 smarlny     1449 mdeleon    [ br  5   0]   3:48 -  4:28 (34-35) W: 20
241 1523 pbanks      1357 jlongshot  [ sr 15   5]   1:20 -  3:46 (23-31) W: 38
438 1393 Allnovice   1498 ftuteleers [ br  3   3]   1:43 -  2:05 (39-39) B: 14
203 1482 AjayNema    1417 kundeomar  [ sr  5  30]   8:18 -  7:34 (38-39) B: 11
147 1279 alza        1631 claytondfo [ br  3   0]   2:11 -  2:30 (32-31) W: 17
296 1454 sciacallo   1463 glensimon  [ br  3   0]   2:52 -  2:50 (39-39) B:  7
398 1448 carlocortom 1491 LeMusa     [ sr 15   0]  11:32 - 11:04 (23-22) W: 25
446 1375 lakicevic   1571 LittleLurK [ br  3   0]   1:35 -  1:32 (33-36) W: 18
247 1539 Lawyers     1413 besenstiel [ br  6  10]   5:08 -  5:57 (32-32) W: 19
378 1483 dbasta      1470 YoMa       [ br  3   0]   1:43 -  1:22 (28-28) B: 25
454 1614 ricardogarv 1342 resol      [ br  3   0]   2:26 -  2:22 (35-35) B:  7
  2 1417 teerexx     1541 nyju       [ sr  5  15]   5:51 -  5:04 (32-34) W: 13
250 1689 kingfishjua 1269 tonysjo    [ br  3   0]   1:26 -  0:05 (10- 8) B: 43
297 1542 pabloarg    1426 Alby       [ br  3   0]   1:08 -  0:43 (20-22) B: 39
152 1484 reiziger    1486 veef       [ br  2   5]   2:21 -  2:01 (38-38) W:  8
276 1380 seabed      1591 ficuest    [ br  2  12]   3:02 -  3:09 (35-36) W: 11
 84 1375 Bakshi      1601 whyhandle  [ br  5   0]   4:11 -  2:59 (35-35) B: 21
169 1532 lifeonmars  1444 pmohanty   [ sr 30   0]  30:00 - 30:00 (39-39) W:  1
 30 1473 tynewydd    1508 LeapingLeo [ sr 15  20]  10:35 - 13:09 (35-36) W: 23
 80 1704 mscp        1279 RDanneskjo [ br  5   0]   5:00 -  5:00 (39-39) W:  1
416 1503 Cantabrico  1492 guire      [ sr 15   0]  14:01 - 13:47 (25-25) W: 14
420 1391 sabirr      1625 farsed     [ sr 15   0]  10:59 - 10:30 (29-28) B: 24
304 1459 alexgob     1558 mindlin    [ br  5   0]   2:15 -  2:37 (35-34) B: 25
337 1576 LALPA       1448 jrmchess   [ br  3   0]   2:04 -  1:45 (25-27) B: 19
321 1530 jvarandas   1503 DixierRebe [ br  3   0]   1:58 -  1:55 (16-15) B: 33
287 1478 UrGameOver  1559 prickler   [ br  3   0]   2:40 -  2:18 (32-32) B: 17
173 1579 MohdRafi    1462 lesterade  [ br  5   0]   2:02 -  3:09 ( 8- 5) B: 36
366 1566 COMTIBoy    1483 mikot      [ br  3   0]   1:45 -  1:56 (18-23) B: 25
345 1457 pfoquet     1596 nichtverge [ br  3   0]   1:51 -  1:27 (33-34) B: 18
370 1453 McGuffinn   1600 pedreiro   [ br  3   0]   1:22 -  1:59 (29-28) B: 21
189 1568 trss        1491 psjw       [ br  3   0]   1:23 -  1:08 (10-10) W: 31
424 1692 drdrunk     1375 nqotheone  [ br  3   0]   1:39 -  1:04 (13-10) B: 31
 50 1402 manel       1669 ybbun      [ br  1   3]   1:13 -  0:48 (27-28) W: 23
400 1493 bladeknife  1589 WillyLPAL  [ br  3   0]   2:56 -  2:56 (39-39) B:  9
204 1499 GrandmaLurK 1584 jajams     [ sr 15   0]   6:34 -  5:15 (21-18) B: 33
377 1536 sergiusebi  1567 mariansaba [ br  5   0]   4:48 -  4:56 (39-39) W:  5
336 1529 JoeO        1588 Goodknight [ br  5   2]   4:35 -  4:28 (39-39) W:  7
  8 1517 Forta       1603 penwell    [ br  5   0]   1:20 -  3:07 (32-32) B: 26
254 1525 anuraganura 1605 Inesperto  [ sr 15   0]   5:24 -  8:05 (23-26) W: 24
198 1572 asciiman    1564 timeover   [ br  3   0]   0:25 -  0:50 ( 3- 6) B: 54
433 1378 Pushkin     1758 balard     [ br  5   0]   2:49 -  2:34 (24-24) B: 23
342 1554 molkin      1590 johoy      [ br  5   3]   1:19 -  0:53 ( 7- 9) B: 46
115 1501 bergin      1645 wshell     [ br  5   0]   5:00 -  5:00 (39-39) B:  1
178 1575 Tjoo        1573 haythammed [ sr 15  30]  15:33 - 16:08 (38-38) W:  8
340 1603 VPolevikov  1548 Rodolfo    [ br  5   0]   3:41 -  3:13 (38-38) W: 22
243 1524 jlatifwrigh 1632 jagbalsing [ sr 15   0]   8:16 -  7:57 (35-36) B: 16
316 1531 Dubina      1634 yakovgreen [ br  5   0]   3:01 -  3:03 (22-22) W: 28
134 1660 scottab     1507 Chaoswarri [ br  5   5]   1:47 -  2:37 (17-18) B: 33
410 1699 bodblugh    1475 rriberi    [ br  2   2]   1:43 -  1:22 (38-38) B: 13
179 1634 Jagga       1541 anjor      [pbr  3   0]   1:56 -  2:18 (33-30) B: 17
221 1357 latmanj     1820 Guimasbec  [ br  5   0]   2:55 -  3:21 (35-35) B: 22
174 1573 Federalist  1608 rmmsf      [ sr 12  15]  10:45 - 12:19 (33-29) B: 20
 23 1558 MaciejMan   1627 schlernap  [ br  3   0]   2:26 -  2:37 (35-35) W: 13
425 1666 dadatiti    1523 allegories [ br  3   0]   1:35 -  0:41 (35-35) W: 20
192 1670 sebcole     1525 TheBithShu [ lr  1   0]   1:00 -  1:00 (39-39) W:  1
380 1629 zulugodetia 1569 pmviva     [ sr 20  20]  18:36 - 27:30 (11-14) W: 29
 33 1619 JonyGuitar  1591 maskarado  [ br  5   0]   2:54 -  2:37 (13-14) W: 27
109 1585 Paquitonune 1631 aSpAdA     [ sr 15   0]  13:30 - 14:39 (39-39) W:  6
362 1616 mastermonst 1601 NightEagle [ lr  1   0]   0:17 -  0:15 (19-19) W: 30
 61 1569 Tnot        1675 robtopliss [ sr 15   0]  13:24 - 12:52 (23-29) B: 22
  5 1625 swolf       1626 EricAnn    [ br  3   0]   2:41 -  2:51 (38-38) W: 10
 78 1658 steustache  1597 Zuvielisat [ br  5   0]   2:56 -  2:45 (15-13) B: 35
456 1630 famoxy      1630 iallip     [ br  3   0]   2:14 -  2:44 (28-33) W: 19
 12 1628 diablorey   1641 danielboon [ br  5   3]   4:20 -  4:36 (34-35) W: 17
285 1655 frippertron 1620 sandel     [ br  5   0]   4:25 -  3:53 (35-35) B: 16
294 1625 javideep    1659 bucalka    [ br  5   0]   4:33 -  4:49 (37-38) W:  7
232 1597 FEDERSCACCH 1696 RostyTosty [ br  3   0]   1:46 -  1:20 (30-31) W: 22
282 1629 tihibuda    1666 ittogami   [ br  3   2]   0:48 -  0:48 (20-20) W: 35
418 1671 filco       1629 joselira   [ sr 15  12]   8:54 - 19:59 ( 7-10) B: 50
353 1619 stefoo      1690 PatuliNois [ br  3   0]   2:45 -  2:53 (35-35) B: 10
307 1857 defilades   1863 DonToemmel [ Br  2   0]   1:14 -  1:01 (43-44) W: 20
355 1568 pgkaralis   1347 salmahosam [ Br  2   0]   1:09 -  1:14 (35-34) W: 23
223 1661 Nangali     1662 MauriJ     [ sr 30   0]  20:46 - 24:07 (22-19) W: 22
 56 1579 aksay       1756 Lordofwar  [ su 18  18]  16:19 - 20:51 ( 5- 7) W: 44
448 1802 MaestroAndT 1534 CaryPaul   [pbr  3   0]   2:54 -  3:00 (39-39) W:  3
 19 1689 KIAUA       1653 truepatefo [ sr 15   0]  13:53 - 14:10 (38-38) W: 11
 32 1665 agavefish   1688 mairipo    [ br  3   0]   1:48 -  0:57 (19-13) B: 37
333 1717 Osolemirnix 1642 marlonlp   [ br  5   0]   1:29 -  3:44 (28-28) B: 24
236 1682 harlemknigh 1680 tuffshaq   [ sr 15   0]   7:34 -  7:59 (22-23) W: 30
 65 1704 KingCobraZ  1661 Quwhy      [ sr 15   0]   7:08 -  7:34 (30-31) B: 31
407 1643 theprof     1723 snthor     [ lr  1   0]   0:54 -  0:52 (35-32) B:  9
175 1793 Gerkl       2125 Caminator  [ Br  2   0]   0:54 -  1:19 (31-40) W: 19
327 1394 GUILTYKING  1455 ccmandolin [ Br  2   0]   0:54 -  1:20 (47-38) W: 18
363 1368 wycliff     2016 BamBouTige [ lr  1   0]   0:14 -  0:35 (18-20) B: 26
195 1765 Filozoltan  1634 jerkus     [ sr 15   0]   0:51 -  0:40 (21- 0) B: 65
157 1799 Lexmo       1608 indianpool [ br  3   0]   1:43 -  1:41 (26-26) W: 27
290 1781 vjd         1634 Phyaeth    [ br  3   0]   1:01 -  1:00 (15-18) W: 42
299 1780 allenton    1635 jobzaz     [ sr 20   0]  16:19 - 15:20 (24-23) B: 14
 70 1727 xandor      1689 Eupator    [ br  3   0]   2:37 -  2:19 (38-38) W: 12
392 1723 aherring    1704 innlay     [ br  5   0]   0:24 -  2:01 (20-27) W: 32
122 1870 markWR      1561 UrsPanda   [ br  5   0]   3:40 -  4:30 (32-32) W: 17
397 1766 foobaer     1701 beercan    [ br  5   0]   4:12 -  4:21 (28-28) B: 12
 87 1752 Vrikolakas  1725 thomass    [ br  5   0]   5:00 -  5:00 (39-39) B:  1
185 1747 MrBoston    1730 Krsh       [ sr 15   0]  11:00 -  1:51 (28-28) W: 40
405 1809 gorom       1673 PreZandy   [ sr 10  10]   9:38 -  8:59 (31-28) B: 14
332 1787 franksama   1700 fluters    [ br  3   0]   2:29 -  2:27 (33-32) W: 15
328 1780 brumel      1714 Cheewaka   [ sr 15   0]  13:31 - 13:23 (33-33) B: 15
368 2018 foggydew    1485 osheter    [ br  3   0]   2:12 -  2:24 (38-36) W: 14
 88 1679 SOUTHBEACH  1827 exetherMEG [ lr  1   0]   0:54 -  0:55 (29-29) W: 10
280 1986 blore       1525 germanym   [ sr 15   0]  14:58 - 14:38 (39-39) B:  2
268 1776 newbyegm    1743 rzviel     [ sr 15   0]  10:14 -  7:49 (25-27) W: 24
289 1718 Demicka     1801 Axe        [ br  3   0]   2:09 -  2:00 (20-21) W: 23
116 1825 Dimosthenis 1701 Sullyman   [ sr 15   0]   4:34 -  3:21 ( 8- 7) B: 68
357 1808 AlteSchlunz 1736 mshadyboy  [ br  3   0]   2:43 -  2:23 (35-35) B: 14
434 1771 rodnojo     1782 gservat    [ br  3   0]   1:58 -  1:30 (29-28) W: 19
 42 1811 GrandpaLurK 1749 serafimcoj [ sr 15   0]   3:37 -  2:40 (18-16) B: 47
111 1265 SusiR       2297 oldman     [ br  5   0]   2:19 -  2:22 (24-25) W: 34
393 1719 alemanguayo 1862 Textorius  [ bu  3   0]   2:29 -  2:19 (35-35) W: 18
262 1864 Pandolfi    1720 BugMare    [ br  3   0]   0:21 -  0:30 ( 9- 9) B: 45
358 1749 Maltietis   1842 schachgott [ br  3   0]   2:51 -  2:49 (36-30) B:  7
 24 1782 cbu         1816 PLAYERFORE [ br  3   0]   0:20 -  0:07 ( 8- 7) W: 42
244 1644 GerryScott  1954 mandevil   [ br  3   0]   1:32 -  2:04 (11-12) W: 25
164 1891 alextheseam 1718 potentia   [ sr 15   0]  11:31 - 10:46 (35-35) W: 17
 69 1805 realface    1811 gruffaloso [ br  3   0]   3:00 -  3:00 (39-39) W:  1
408 1803 PasVuPasPri 1817 yelloman   [ lr  1   0]   0:24 -  0:28 (31-31) W: 23
 41 1586 Kaitlin     2036 twizter    [ sr 30   5]  28:45 - 25:24 (29-32) B: 15
443 1875 TresMinutos 1747 kanz       [ br  3   0]   2:34 -  2:20 (35-35) B: 11
431 1899 psyvarriar  1751 Leader     [ br  3   0]   1:13 -  1:00 (28-24) B: 25
172 1809 kuchuberia  1846 daziem     [ sr 15   0]   1:41 -  0:00 (18-12) W: 46
440 1846 Biber       1818 travino    [ sr 15   0]   2:40 -  9:35 (33-34) W: 35
212 1816 ConverFIC   1852 alfagonzal [ br  5   0]   0:58 -  0:41 (25-32) W: 37
388 1893 Yuffy       1779 scrunchey  [ lr  1   0]   0:52 -  0:58 (38-37) B:  8
 76 1719 daroverg    1956 chesspickl [ br  5   0]   4:51 -  4:39 (38-38) B:  4
 66 1766 WoodDemon   1918 agoraphobi [ br  3   0]   1:30 -  0:59 (13-12) B: 31
 62 1907 RJx         1781 skunkmaste [ zr  3   0]   0:05 -  0:37 (54-24) B: 48
 89 1902 lexterrozy  1808 ponmeuno   [ sr 15   0]  13:33 - 13:28 (38-38) B: 16
390 1839 lanandt     1871 astronomy  [ br  3   0]   0:15 -  1:02 (16-16) B: 38
205 1882 alekhinus   1832 chamerion  [ sr 30  20]  26:57 - 28:06 (28-32) B: 21
 47 1791 Kaliumcyani 1924 JuniorLurK [ br  5   0]   1:08 -  1:03 (10- 9) B: 42
317 1874 Troisvierge 1850 michellebl [ sr 45  45]  47:27 - 31:05 (23-25) B: 22
 86 1847 ScorpionKni 1886 BexBex     [ sr 45   2]  27:11 - 28:18 (19-23) W: 22
444 1955 maassluis   1793 Motherhack [ lr  1   0]   0:54 -  0:58 (37-38) W:  9
430 1911 DJWilson    1855 sleppup    [ sr 15   0]   7:10 -  2:04 (22-22) W: 35
208 1910 CABAMECAP   1873 aspecialon [ br  3   0]   2:41 -  2:27 (32-32) B: 18
272 2009 Obliviax    1817 salustiano [ br  3   0]   0:28 -  1:15 (23-29) B: 33
 51 1983 Stellardron 1874 martinvoor [ br  3   0]   0:39 -  0:37 (17-17) B: 27
112 1927 GriffyJr    1932 gromarx    [ br  2  12]   8:20 -  1:13 (21-22) B: 33
202 2007 StickyPawn  1862 Kamikazepa [ sr 15   0]   6:15 -  9:52 (27-27) W: 27
108 1891 ScienceFict 1982 OneFromNon [ br  3   0]   2:43 -  2:51 (38-38) B: 10
295 2026 xivarmy     1850 Pcibiri    [ sr 45  45]  46:01 - 45:32 (39-39) B:  9
432 1909 shinloom    1971 Terrapin   [ br  3   0]   1:10 -  1:06 (27-27) B: 21
360 2007 Filosofo    1951 Renevil    [ br  3   0]   1:49 -  1:37 (30-30) W: 24
252 1973 tizgran     2016 Papatry    [ br  3   0]   1:29 -  1:36 (29-28) B: 20
126 2064 Chezac      1966 SKC        [ lr  1   0]   0:44 -  0:43 (29-29) W: 15
 90 2039 Ovnis       2034 CedAndPiez [ sr 15   0]   7:17 -  5:13 (20-19) B: 33
381 1962 gasteiz     2126 AlastorM   [ br  3   0]   0:26 -  0:47 (19-21) W: 28
158 1944 SportClubRe 2147 OLDENGAWY  [ sr 45  45]  45:46 - 48:34 (38-38) B:  8
326 1852 WilkBardzoZ 2267 zurichess  [ sr 15  15]   8:48 - 13:39 (35-35) W: 11
266 2049 CatNail     2072 ohmanbruin [ Su  3   0]   2:57 -  2:29 (10- 8) B: 16
309 2061 axeltiger   2082 milpat     [ sr 45  45]  24:21 - 15:03 (39-39) B: 15
 58 1807 karnij      2339 Thukydides [ br  3   0]   2:57 -  3:00 (39-39) W:  4
452 2116 Haithamyous 2156 darkiest   [ sr 15   5]  14:36 - 14:22 (36-36) W:  8

  453 games displayed.
";
            var games = FicsClient.ParseGames(GamesString);

            Assert.IsNotNull(games);
            Assert.AreEqual(games.Count, 455);
        }
    }
}

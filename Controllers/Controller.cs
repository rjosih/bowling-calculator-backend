using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace QuickStart.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalculateController : ControllerBase
    {
        private IService service;

        private readonly ILogger<CalculateController> logger;
        private readonly IMapper<PlayerRequestModel, Player> playerMapper;
        private readonly IMapper<RequestModel, InputModel> inputModelMapper;
        private readonly IMapper<OutputModel, ResponseModel> responseModelMapper;

        public CalculateController(ILogger<CalculateController> logger)
        {
            this.logger = logger;
            this.service = new Service(logger);
            this.playerMapper = new PlayerMapper();
            this.inputModelMapper = new InputModelMapper();
            this.responseModelMapper = new ResponseModelMapper();
        }

        [HttpPost]
        public dynamic Post(RequestModel requestModel)
        {
            this.logger.LogInformation($"{requestModel.ToString()}");

            try
            {
                var inputModel = this.inputModelMapper.Map(requestModel);
                var outputModel = updateCurrentRoll(requestModel.rolls, requestModel.lastScore);
                this.logger.LogInformation($"{outputModel}");
                return outputModel;
            }
            catch (System.Exception)
            {
                throw;
            }
        }


        bool isStrike(int pins)
        {
            return Convert.ToInt32(APIScoreWay.Strike) == pins;
        }

        bool isEven(int number)
        {
            return number % 2 == 0;
        }


        // frames --> an arary with  arrays of two, the rounds [[3,2], [4,4]] (are being shown and done after one frame round)

        int getFrameIndex(List<int[]> frames)
        {
            return frames.Count - 1;
        }

        bool isSpare(int roll1, int roll2)
        {
            var total = roll1 + roll2;
            return total == 10;
        }

        bool isBonusRoll(int rolls)
        {
            var bonusRoll = 20;
            return rolls == bonusRoll;
        }




        int strikeBonus(int roll1, int roll2)
        {
            var total = roll1 + roll2;
            return 10 + total;
        }

        void increaseOne (ref int i) {
            i++;
        }



        [HttpPost("updateCurrentRoll")]
        public int updateCurrentRoll(int rolls, int lastScore)
        {
            if (isStrike(lastScore) && isEven(rolls) && rolls < 18)
            {
                return rolls + 2;
            }
            else
            {
                int newRolls = rolls + 1;
                return newRolls;
            }
        }


        [HttpPost("isGameOver")]
        public bool isGameOver([FromBody] RequestModel requestData)
        {
            int rolls = requestData.rolls;
            int lastScore = requestData.lastScore;

            List<int[]> pins = new List<int[]>();
            pins = requestData.pins;

            bool GameNotOver =
              rolls < 19 || (rolls == 19 && (isSpare(lastScore, pins.LastOrDefault()[0]) || isStrike(pins.LastOrDefault()[0])));
            return !GameNotOver;
        }

        [HttpPost("updateFrames")]
        public dynamic updateFrames([FromBody] RequestModel requestData)
        {
            int rolls = requestData.rolls;
            int lastScore = requestData.lastScore;

            List<int[]> frames = new List<int[]>();
            frames = requestData.frames;

            if (isEven(rolls) && !isBonusRoll(rolls))
            {
                //lastScore 4
                //frames[[4,2],[10][3,] <-- doesn't show not finished
                // --> frames[[4,2],[10] [3.4]

                frames.Add(new int[] { lastScore });
                return frames;
            }
            else
            {
                var frameList = frames.LastOrDefault().Concat(new int[] { lastScore }).ToArray();
                frames.RemoveAt(getFrameIndex(frames));
                frames.Add(frameList);
                return frames;
            }
        }

        [HttpPost("updateCumulativeScore")]
        public int[] updateCumulativeScore([FromBody] RequestModel requestData)
        {
            int rolls = requestData.rolls;
            int lastScore = requestData.lastScore;

            List<int[]> frames = new List<int[]>();
            frames = requestData.frames;

            int[] cumulativeScores;
            cumulativeScores = requestData.cumulativeScores;

            List<int[]> pins = new List<int[]>();
            pins = requestData.pins;

            // rolls --> "roles"
            // frames --> an arary with  arrays of two, the rounds [3,2] 
            // cumulativeScores --> array with all the totSums for each frames
            // pins --> array with all the rounds and their values [2, 3, 4, 5] on a led 
            // last score --> latest score


            // Takes the last value in the totalSum array
            var currentScore = cumulativeScores.LastOrDefault() | 0;

            if ((!isEven(rolls) && !isStrike(lastScore) && !isSpare(pins.LastOrDefault()[0], lastScore)) || isBonusRoll(rolls))
            {
                var lastElement = frames.LastOrDefault();
                int frameScore = isBonusRoll(rolls) ?
                  lastElement[lastElement.Length - 1] + lastElement[lastElement.Length - 2] + lastScore
                  : lastElement[lastElement.Length - 1] + lastScore;

                if (isStrike(pins.LastOrDefault()[0]) && !isStrike(pins[pins.Count - 2][0]) && rolls == 19)
                    return cumulativeScores;

                if (isStrike(pins[pins.Count - 2][0]) && rolls > 2 && rolls < 20)
                {
                    int bonus = strikeBonus(pins.LastOrDefault()[0], lastScore);
                    int previousFrame = bonus + currentScore;

                    if (isStrike(pins.LastOrDefault()[0]) && rolls == 19)
                    {
                        cumulativeScores.Append(previousFrame);
                    }
                    else
                    {
                        cumulativeScores.Append(previousFrame);
                        cumulativeScores.Append(frameScore + previousFrame);
                    }

                    return cumulativeScores;
                }

                int[] updatedFrameScores = new int[] { };
                for (int i = 0; i < cumulativeScores.Length; i++)
                {
                    updatedFrameScores.Append(cumulativeScores[i]);
                }
                updatedFrameScores.Append(currentScore + frameScore);
                return updatedFrameScores;

            }
            else if (isStrike(pins[pins.Count - 2][0]) && rolls > 2 && rolls < 20)
            {
                int bonus = strikeBonus(pins.LastOrDefault()[0], lastScore);
                cumulativeScores.Append(currentScore + bonus);
                return cumulativeScores;
            }
            else if (isEven(rolls) && isSpare(pins[pins.Count - 2][0], pins.LastOrDefault()[0]))
            {
                int spareFrame = 10 + lastScore;
                cumulativeScores.Append(currentScore + spareFrame);
                return cumulativeScores;
            }
            return cumulativeScores;
        }
    }
}
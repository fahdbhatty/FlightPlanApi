using FlightPlanApi.Data;
using FlightPlanApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanApi.Controllers
{
    [Route("api/v1/flightplan")]
    [ApiController]
    public class FlightPlanController : ControllerBase
    {
        private IDatabaseAdapter _database;

        public FlightPlanController(IDatabaseAdapter database)
        {
            _database = database;
        }

        [HttpGet]
        public async Task<IActionResult> FlightPlanList()
        {
            var flightPlanList = await _database.GetAllFlightPlans();
            if (flightPlanList.Count == 0)
                return NoContent(); //return 204
            return Ok(flightPlanList); //returns 200 and flight plan list in body
        }

        [HttpGet]
        [Route("{flightPlanId}")]
        public async Task<IActionResult> GetFlightPlanById(string flightPlanId)
        {
            var flightPlan = await _database.GetFlightPlanById(flightPlanId);
            if (flightPlan.FlightPlanId != flightPlanId)
                return StatusCode(StatusCodes.Status404NotFound);
            return Ok(flightPlan);
        }

        [HttpPost]
        [Route("file")] //file is part of the route path. it isnt a variable or any special flag or anything
        public async Task<IActionResult> FileFlightPlan(FlightPlan flightPlan)
        {
            var transactionResult = await _database.FileFlightPlan(flightPlan);
            switch (transactionResult)
            {
                case TransactionResult.Success:
                    return Ok();
                case TransactionResult.BadRequest:
                    return StatusCode(StatusCodes.Status400BadRequest);
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateFlightPlan(FlightPlan flightPlan)
        {
            var transactionResult = await _database.FileFlightPlan(flightPlan);
            switch (transactionResult)
            {
                case TransactionResult.Success:
                    return Ok();
                case TransactionResult.BadRequest:
                    return StatusCode(StatusCodes.Status400BadRequest);
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete]
        [Route("{flightPlanId}")]
        public async Task<IActionResult> DeleteFlightPlan(string flightPlanId)
        {
            var result = await _database.DeleteFlightPlanById(flightPlanId);
            if (result)
                return Ok();
            return StatusCode(StatusCodes.Status404NotFound);
        }

        [HttpGet]
        [Route("airport/departure/{flightPlanId}")]
        public async Task<IActionResult> GetFlightPlanDepartureAirport(string flightPlanId)
        {
            var flightPlan = await _database.GetFlightPlanById(flightPlanId);
            if (flightPlan.FlightPlanId != flightPlanId)
                return StatusCode(StatusCodes.Status404NotFound);
            return Ok(flightPlan.DepartureAirport);
        }

        [HttpGet]
        [Route("route/{flightPlanId}")]
        public async Task<IActionResult> GetFlightPlanRoute(string flightPlanId)
        {
            var flightPlan = await _database.GetFlightPlanById(flightPlanId);
            if (flightPlan.FlightPlanId != flightPlanId)
                return StatusCode(StatusCodes.Status404NotFound);
            return Ok(flightPlan.Route);
        }

        [HttpGet]
        [Route("time/enroute/{flightPlanId}")]
        public async Task<IActionResult> GetFlightPlanTimeEnroute(string flightPlanId)
        {
            var flightPlan = await _database.GetFlightPlanById(flightPlanId);
            if (flightPlan.FlightPlanId != flightPlanId)
                return StatusCode(StatusCodes.Status404NotFound);

            var estimatedTimeRoute = flightPlan.ArrivalTime - flightPlan.DepartureTime;
            return Ok(estimatedTimeRoute); 
        }
    }
}

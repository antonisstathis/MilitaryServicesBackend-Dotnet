using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilitaryServices.App.Dto;
using MilitaryServices.App.Entity;
using MilitaryServices.App.Enums;
using MilitaryServices.App.Security;
using MilitaryServices.App.Service;
using MilitaryServices.App.Services;
using MilitaryServicesApp.Service;
using MilitaryServicesBackendDotnet.Security;
using System.Globalization;
using System.Security.Claims;
using System.Text.Json;

namespace MilitaryServices.App.Controllers
{
    [Authorize(Roles = "SOLDIER")]
    [ApiController]
    [Route("[controller]")]
    public class SoldiersController : ControllerBase
    {
        private readonly ISoldierService _soldierService;
        private readonly ISerOfUnitService _serOfUnitService;
        private readonly IUserService _userService;
        private readonly IJwtUtil _jwtUtil;
        private readonly UserPermission _userPermission;
        private readonly IMessageService _messageService;

        public SoldiersController(
            ISoldierService soldierService,
            SerOfUnitService serOfUnitService,
            UserService userService,
            IMessageService messageService,
            UserPermission userPermission)
        {
            _soldierService = soldierService;
            _serOfUnitService = serOfUnitService;
            _userService = userService;
            _messageService = messageService;
            _userPermission = userPermission;
        }

        private string GetUsername() => User.FindFirstValue(ClaimTypes.Name);

        [HttpGet("getSoldiers")]
        public IActionResult GetSoldiers()
        {
            var username = SanitizationUtil.Sanitize(GetUsername());
            var soldiers = _soldierService.FindAll(username)
                .Select(s => new SoldierDto(
                    s.Token,
                    SanitizationUtil.Sanitize(s.Company),
                    SanitizationUtil.Sanitize(s.Name),
                    SanitizationUtil.Sanitize(s.Surname),
                    SanitizationUtil.Sanitize(s.Situation),
                    SanitizationUtil.Sanitize(s.Active),
                    SanitizationUtil.Sanitize(s.Service),
                    s.ExtractDate(),
                    SanitizationUtil.Sanitize(s.Armed)
                )).ToList();

            return Ok(soldiers);
        }

        [HttpGet("getSoldiersOfUnit")]
        public IActionResult GetSoldiersOfUnit()
        {
            var username = SanitizationUtil.Sanitize(GetUsername());
            var soldiers = _soldierService.LoadSoldiers(username)
                .Select(s => new SoldierPersonalDataDto
                {
                    Token = s.Token,
                    SoldierRegistrationNumber = s.SoldierRegistrationNumber,
                    Company = SanitizationUtil.Sanitize(s.Company),
                    Name = SanitizationUtil.Sanitize(s.Name),
                    Surname = SanitizationUtil.Sanitize(s.Surname),
                    Situation = SanitizationUtil.Sanitize(s.Situation),
                    Active = SanitizationUtil.Sanitize(s.Active),
                    Discharged = SanitizationUtil.Sanitize(s.Discharged),
                    Patronymic = SanitizationUtil.Sanitize(s.Patronymic),
                    Matronymic = SanitizationUtil.Sanitize(s.Matronymic),
                    MobilePhone = SanitizationUtil.Sanitize(s.MobilePhone),
                    City = SanitizationUtil.Sanitize(s.City),
                    Address = SanitizationUtil.Sanitize(s.Address)
                }).ToList();

            return Ok(soldiers);
        }

        [HttpGet("getSoldierByRegistrationNumber")]
        public IActionResult GetSoldierByRegistrationNumber([FromQuery] string regnumb)
        {
            var soldiers = _soldierService.FindSoldiersByRegistrationNumber(regnumb)
                .Select(s => new SoldierPersonalDataDto
                {
                    Token = s.Token,
                    SoldierRegistrationNumber = s.SoldierRegistrationNumber,
                    Company = SanitizationUtil.Sanitize(s.Company),
                    Name = SanitizationUtil.Sanitize(s.Name),
                    Surname = SanitizationUtil.Sanitize(s.Surname),
                    Situation = SanitizationUtil.Sanitize(s.Situation),
                    Active = SanitizationUtil.Sanitize(s.Active),
                    Discharged = SanitizationUtil.Sanitize(s.Discharged),
                    Patronymic = SanitizationUtil.Sanitize(s.Patronymic),
                    Matronymic = SanitizationUtil.Sanitize(s.Matronymic),
                    MobilePhone = SanitizationUtil.Sanitize(s.MobilePhone),
                    City = SanitizationUtil.Sanitize(s.City),
                    Address = SanitizationUtil.Sanitize(s.Address)
                }).ToList();

            return Ok(soldiers);
        }

        [HttpGet("getServicesOfSoldier")]
        public async Task<IActionResult> GetServicesOfSoldier([FromQuery] string soldierToken)
        {
            var username = _jwtUtil.ExtractUsername(Request);
            var user = _userService.FindUser(username);

            if (user == null)
                return Unauthorized();

            var soldierIdStr = _jwtUtil.ExtractUsername(soldierToken);
            if (!int.TryParse(soldierIdStr, out int soldierId))
                return BadRequest("Invalid soldierToken");

            var unit = user.Soldier.Unit;

            var services = _soldierService.FindServicesOfSoldier(unit, soldierId);

            // Sanitize the data
            var sanitizedServices = services.Select(service => new ServiceDto(
                service.Id,
                SanitizationUtil.Sanitize(service.Service),
                service.ServiceDate,
                SanitizationUtil.Sanitize(service.Armed),
                SanitizationUtil.Sanitize(service.Description),
                SanitizationUtil.Sanitize(service.Shift)
            )).ToList();

            return Ok(sanitizedServices);
        }

        [HttpGet("dischargeSoldier")]
        public async Task<IActionResult> DischargeSoldier([FromQuery] string soldierToken)
        {
            var username = _jwtUtil.ExtractUsername(Request);
            var user = _userService.FindUser(username);
            if (user == null)
                return Unauthorized();

            var unit = user.Soldier.Unit;
            var soldierIdStr = _jwtUtil.ExtractUsername(soldierToken);
            if (!int.TryParse(soldierIdStr, out int soldierId))
                return BadRequest("Invalid soldierToken");

            bool result = _soldierService.DischargeSoldier(soldierId, unit);
            if (result)
            {
                var message = _messageService.GetMessage(MessageKey.DISCHARGE_SOLDIER_SUCCESSFUL, CultureInfo.GetCultureInfo("en"));
                return Ok(message);
            }
            else
            {
                var message = _messageService.GetMessage(MessageKey.DISCHARGE_SOLDIER_NOT_PERMITTED, CultureInfo.GetCultureInfo("en"));
                return Unauthorized(message);
            }
        }

        [HttpGet("getFirstCalcDate")]
        public IActionResult GetFirstCalcDate()
        {
            string sanitizedUsername = SanitizationUtil.Sanitize(_jwtUtil.ExtractUsername(Request));
            DateTime dateOfFirstCalc = _soldierService.GetDateByCalculationNumber(sanitizedUsername, 1);

            return Ok(dateOfFirstCalc);
        }

        [HttpGet("getPreviousCalculation")]
        public async Task<IActionResult> GetPreviousCalculation([FromQuery] string date)
        {
            if (!DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var prevDate))
            {
                return BadRequest("Invalid date format, expected yyyy-MM-dd");
            }

            string sanitizedUsername = SanitizationUtil.Sanitize(_jwtUtil.ExtractUsername(Request));
            List<SoldierPreviousServiceDto> soldiers = _soldierService.FindPreviousCalculation(sanitizedUsername, prevDate);
            var sanitizedSoldiers = soldiers.Select(soldier => new SoldierPreviousServiceDto(
                soldier.Token,
                SanitizationUtil.Sanitize(soldier.SoldierRegistrationNumber),
                SanitizationUtil.Sanitize(soldier.Company),
                SanitizationUtil.Sanitize(soldier.Name),
                SanitizationUtil.Sanitize(soldier.Surname),
                SanitizationUtil.Sanitize(soldier.Situation),
                SanitizationUtil.Sanitize(soldier.Active),
                SanitizationUtil.Sanitize(soldier.Service),
                soldier.Date,
                soldier.Armed,
                SanitizationUtil.Sanitize(soldier.Discharged)
            )).ToList();

            return Ok(sanitizedSoldiers);
        }

        [HttpGet("calc")]
        public IActionResult CalculateNewServices([FromQuery] DateTime lastDate)
        {
            string Username = SanitizationUtil.Sanitize(_jwtUtil.ExtractUsername(Request));
            _soldierService.CalculateServices(Username,lastDate);
            var message = _messageService.GetMessage(MessageKey.NEW_SERVICES_CALCULATED, CultureInfo.GetCultureInfo("en"));

            return Ok(message);
        }

        [HttpGet("getServices")]
        public async Task<IActionResult> GetServices([FromQuery] string? date = null)
        {
            var username = _jwtUtil.ExtractUsername(Request);
            var user = _userService.FindUser(username);
            DateTime? parsedDate = null;
            if (DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                parsedDate = dt;
            else
                return BadRequest("Invalid date format, expected yyyy-MM-dd");
            var services = _serOfUnitService.GetAllServices(user.Soldier.Unit, parsedDate);
            return Ok(services);
        }

        [HttpGet("getNameOfUnit")]
        public async Task<IActionResult> GetNameOfUnit()
        {
            var username = SanitizationUtil.Sanitize(_jwtUtil.ExtractUsername(Request));
            var user = _userService.FindUser(username);
            var unitName = SanitizationUtil.Sanitize(user.Soldier.Unit.NameOfUnit);

            return Ok(unitName);
        }

        [HttpGet("getSoldiersStatistics")]
        public async Task<IActionResult> GetStatistics([FromQuery] StatisticalData statisticalDataOption)
        {
            var username = _jwtUtil.ExtractUsername(Request);
            var user = _userService.FindUser(username);
            var stats = _soldierService.GetSoldierServiceStats(user.Soldier.Unit, statisticalDataOption);

            return Ok(stats);
        }

        [HttpPost("getSoldier")]
        public async Task<IActionResult> GetSoldier([FromBody] JsonElement sold)
        {
            try
            {
                if (!sold.TryGetProperty("token", out var tokenElement))
                    return BadRequest("Missing token");
                string token = tokenElement.GetString() ?? string.Empty;
                var soldierIdStr = _jwtUtil.ExtractUsername(token);
                if (!int.TryParse(soldierIdStr, out int id))
                    return BadRequest("Invalid token");
                SoldierUnitDto soldierDto = _soldierService.FindSoldier(id);

                // Check if user has access
                bool userHasAccess = _userPermission.CheckIfUserHasAccess(token, Request, soldierDto.Situation, soldierDto.Active);
                if (!userHasAccess)
                {
                    var message = _messageService.GetMessage(MessageKey.UNAUTHORIZED, CultureInfo.GetCultureInfo("en"));
                    return Unauthorized(message);
                }
                // Construct DTO and sanitize string fields
                var soldier = new SoldierDto(
                    SanitizationUtil.Sanitize(soldierDto.Name),
                    SanitizationUtil.Sanitize(soldierDto.Surname),
                    SanitizationUtil.Sanitize(soldierDto.Situation),
                    SanitizationUtil.Sanitize(soldierDto.Active)
                )
                {
                    Token = token,
                };
                soldier.SetDate(DateTime.UtcNow);
                return Ok(soldier);
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid request payload");
            }
        }

        [HttpPost("saveNewSoldier")]
        public IActionResult SaveNewSoldier([FromBody] SoldierPersonalDataDto soldierDto)
        {
            var username = GetUsername();
            var user = _userService.FindUser(username);
            if (user == null) return Unauthorized();

            var sanitizedSoldier = new SoldierPersonalDataDto
            {
                SoldierRegistrationNumber = SanitizationUtil.Sanitize(soldierDto.SoldierRegistrationNumber),
                Company = SanitizationUtil.Sanitize(soldierDto.Company),
                Name = SanitizationUtil.Sanitize(soldierDto.Name),
                Surname = SanitizationUtil.Sanitize(soldierDto.Surname),
                Discharged = DischargedExtensions.GetDischarged(false),
                Situation = SanitizationUtil.Sanitize(soldierDto.Situation),
                Active = SanitizationUtil.Sanitize(soldierDto.Active),
                Patronymic = SanitizationUtil.Sanitize(soldierDto.Patronymic),
                Matronymic = SanitizationUtil.Sanitize(soldierDto.Matronymic),
                MobilePhone = SanitizationUtil.Sanitize(soldierDto.MobilePhone),
                City = SanitizationUtil.Sanitize(soldierDto.City),
                Address = SanitizationUtil.Sanitize(soldierDto.Address)
            };

            _soldierService.SaveNewSoldier(sanitizedSoldier, user.Soldier.Unit);

            return Ok(_messageService.GetMessage(MessageKey.SOLDIER_SAVED.ToString(), CultureInfo.InvariantCulture));
        }

        [HttpPost("saveNewServices")]
        [Authorize(Roles = "COMMANDER")]
        public async Task<IActionResult> SaveNewServices([FromBody] JsonElement payload)
        {
            try
            {
                if (!payload.TryGetProperty("selectedNumberOfGuards", out var guardsElem) ||
                    !payload.TryGetProperty("nameOfService", out var nameElem) ||
                    !payload.TryGetProperty("armed", out var armedElem) ||
                    !payload.TryGetProperty("description", out var descElem) ||
                    !payload.TryGetProperty("shift", out var shiftElem))
                {
                    return BadRequest("Missing required fields");
                }
                int numberOfGuards = guardsElem.GetInt32();
                string nameOfService = nameElem.GetString() ?? "";
                string armedStatus = armedElem.GetString() ?? "";
                string description = descElem.GetString() ?? "";
                string shift = shiftElem.GetString() ?? "";
                // Get current user from token
                var username = _jwtUtil.ExtractUsername(Request);
                var user = _userService.FindUser(username);
                if (user == null)
                    return Unauthorized();
                var soldier = user.Soldier;
                var unit = soldier.Unit;
                var serviceOfUnit = new ServiceOfUnit(nameOfService, armedStatus, soldier.Company, description, shift, unit);
                // Check permission to add services
                if (!_serOfUnitService.CheckIfAllowed(unit, numberOfGuards, serviceOfUnit))
                {
                    var msg = _messageService.GetMessage(MessageKey.ADD_SERVICES_REJECTED, CultureInfo.GetCultureInfo("en"));
                    return BadRequest(msg);
                }

                Enumerable.Range(0, numberOfGuards).ToList().ForEach(_ =>
                {
                    var newService = new ServiceOfUnit(nameOfService, armedStatus, soldier.Company, description, shift, unit);
                    _serOfUnitService.SaveService(newService);
                });

                var successMsg = _messageService.GetMessage(MessageKey.ADD_SERVICES, CultureInfo.GetCultureInfo("en"));
                return Ok(successMsg);
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid request payload");
            }
        }

        [HttpPost("deleteServices")]
        [Authorize(Roles = "COMMANDER")]
        public IActionResult DeleteServices([FromBody] JsonElement payload)
        {
            try
            {
                if (!payload.TryGetProperty("ids", out JsonElement idsElement) || idsElement.ValueKind != JsonValueKind.Array)
                    return BadRequest("Invalid or missing 'ids' array.");
                _soldierService.DeleteServices(idsElement);
                var message = _messageService.GetMessage(MessageKey.SERVICES_DELETED, CultureInfo.GetCultureInfo("en"));
                return Ok(message);
            }
            catch (Exception)
            {
                return BadRequest("Failed to delete services.");
            }
        }

        [HttpPost("changeSoldSituation")]
        public IActionResult ChangeSoldierSituation([FromBody] JsonElement sold)
        {
            try
            {
                if (!sold.TryGetProperty("token", out var tokenElement) ||
                    !sold.TryGetProperty("situation", out var situationElement) ||
                    !sold.TryGetProperty("active", out var activeElement) ||
                    !sold.TryGetProperty("name", out var nameElement) ||
                    !sold.TryGetProperty("surname", out var surnameElement))
                {
                    return BadRequest("Missing required fields.");
                }
                var token = tokenElement.GetString();
                var situation = situationElement.GetString();
                var active = activeElement.GetString();
                var name = nameElement.GetString();
                var surname = surnameElement.GetString();
                // Check user access
                var hasAccess = _userPermission.CheckIfUserHasAccess(token, Request, situation, active);
                if (!hasAccess)
                {
                    var unauthorizedMsg = _messageService.GetMessage(MessageKey.UNAUTHORIZED, CultureInfo.GetCultureInfo("en"));
                    return Unauthorized(unauthorizedMsg);
                }
                // Extract soldier ID from token
                int soldierId = int.Parse(_jwtUtil.ExtractUsername(token));
                // Sanitize input and update soldier
                var soldDto = new SoldDto(
                    soldierId,
                    SanitizationUtil.Sanitize(name),
                    SanitizationUtil.Sanitize(surname),
                    situation,
                    active
                );
                _soldierService.UpdateSoldier(soldDto);
                var successMsg = _messageService.GetMessage(MessageKey.SOLDIER_UPDATED, CultureInfo.GetCultureInfo("en"));
                return Ok(successMsg);
            }
            catch (Exception)
            {
                return BadRequest("Failed to update soldier.");
            }
        }

    }
}

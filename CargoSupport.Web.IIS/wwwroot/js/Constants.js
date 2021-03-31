//HTML DOM
const bootstrapSelectDropdownStyle = 'title="Inget valt" class="selectpicker show-tick"';
const bootstrapSelectDropdownStyleWithLiveSearch = `${bootstrapSelectDropdownStyle} data-live-search="true"`;

//REGEX
const afternoonRegex = "^[Kk]\\d\\d\\s";
const morningRegex = "^[Mm]\\d\\d\\s";
const hamtasRegex = "^[Hh][Ää][Mm][Tt][Aa][Ss]";
const returRegex = "^[Rr]\\d\\d\\s";

//API
const todayGraphs = "/api/v1/Analyze/GetTodayGraphs";
const todayGraphsForDriver = "/api/v1/Analyze/GetTodayGraphsForDriver";
const carStats = "/api/v1/Analyze/GetCarStats";
const driverExtendedStats = "/api/v1/Analyze/GetUnderBoss";
const singleDriverExtendedStats = "/api/v1/Analyze/GetSingleDriverUnderBoss";
const singleDriverSimplifiedRecords = "/api/v1/Analyze/GetSimplifiedRecordsForDriver";

//Formtting
const timeFormat = "YYYY-MM-DD";

//Ports
const availablePorts = [24, 26, 27, 29, 31];

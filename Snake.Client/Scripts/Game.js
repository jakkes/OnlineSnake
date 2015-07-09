var Game = function (uiData) {

    _this = this;

    this.canvas = uiData.canvas;
    this.ammo = uiData.ammo;
    this.armor = uiData.armor;
    this.score = uiData.score;
    this.leader = uiData.leader;
    this.leaderboard = uiData.leaderboard;
    this.personal = uiData.personal;
    this.fps = uiData.fps;
    this.boost = uiData.boost;
    this.players = uiData.players;

    this.GameSettings;
    
    this.loopTimer;
    this.fpsTimer;
    this.dataTimer;

    this.GetData = function () {
        $.getJSON("/Game/GetData", "", function (data) {
            
        });
    }

    $.getJSON("/Game/GetSettings", "", function (data) {
        _this.GameSettings = data;
        $.getJSON("/Game/Join", "", function (data) {
            if (data.ConnectionCode == "200") {
                
            }
        });
    });
}
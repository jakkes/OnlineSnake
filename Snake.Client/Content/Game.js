var canvas = document.getElementById("game");
var context = canvas.getContext("2d");

function connect(url, callback) {
    $.ajax({
        url: url,
        timeout:1000,
        success: callback,
        error: function (){
            console.log("Timeout");
        }
    });
}

var loopTimer;
var dataTimer;
var fpsTimer;

var GameSettings;
var PlayerSettings;

PlayerSettings = {
    turnLeft: 37,
    turnRight: 39,
    shoot: 32,
    boost: 38,
    _break: 40
}

function shoot() {
    if ($("#AmmoLb").text() != "0")
        connect("Game/Shoot");
}

function resize() {
    //var size = window.innerHeight > window.innerWidth ? window.innerWidth - 40 : window.innerHeight - 40;
    //$(canvas).css("height",size); $(canvas).css("width",size); $(canvas).css("border-radius", size / 2);
}

function init() {
    resize();
    connect("/Game/GetSettings", function (data) {
        GameSettings = data;
        connect("/Game/Join", function (data) {
            if (data.ConnectionCode == "200") {
                StartLoop();
            }
        });
    });
}

var count = 0;
function StartLoop() {
    dataTimer = setInterval(getData, 2000);
    loopTimer = setInterval(loop, 33);
    fpsTimer = setInterval(function () {
        $("#FpsLb").text(count);
        count = 0;
    }, 1000);
}

var loopDone = true;
function loop() {
    if (loopDone) {
        loopDone = false;

        var turn = "None";
        if (_leftKey) turn = "Left";
        else if (_rightKey) turn = "Right";

        $.ajax({
            url: "/Game/Loop",
            type: "post",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify({
                "Turn": turn,
                "Boost": _boost,
                "Break": _break,
                "Shoot": _shoot
            }),
            timeout: 100,
            success: function (data) {
                _shoot = false;
                draw(data);
            },
            error: function () {
                loopDone = true;
            }
        });
    }
}

function getData() {
    $.get("/Game/GetData", "", function (data) {
        if (data.ConnectionCode != "200")
            return;
        $("#CurrentLeaderLb").text(data.CurrentLeader.Name + " (" + data.CurrentLeader.Score + ")");
        $("#OnlineLb").text(data.OnlineCount);
        $("#PersonalLb").text(data.PersonalHighscore);
        $("#PlayersOnlineLb").text("");
        for (var i = 0; i < data.Players.length; i++) {
            $("#PlayersOnlineLb").append("<p style=\"color:" + data.Players[i].Color + ";\">" + data.Players[i].Name + "</p>");
        }
        $("#Leaderboard").text("");
        for (var i = 0; i < data.Leaderboard.length; i++) {
            $("#Leaderboard").append("<p>" + (i+1) + ". " + data.Leaderboard[i].Name + " (" + data.Leaderboard[i].Score + ")</p>");
        }
    });
};

function tX(x) {
    return x + 500;
}

function tY(y) {
    return y + 500;
}

function draw(d) {
    try {

        if (data.ConnectionCode == "404") {
            _this.GameOver();
            return;
        } else if (data.ConnectionCode != "200") {
            _this.Stop();
            return;
        }

        $("#ScoreLb").text(d.Score);
        $("#AmmoLb").text(d.AmmoCount);
        $("#ArmorLb").text(d.PlayerArmor);
        $("#BoostLb").text(Math.round(d.BoostStored / 100) + "/" + GameSettings.BOOST_CAP / 100);
        $("#BreakLb").text(Math.round(d.BreakStored / 100) + "/" + GameSettings.BOOST_CAP / 100);
        context.clearRect(0, 0, canvas.width, canvas.height);

        for (var i = 0; i < d.Borders.length; i++) {
            context.beginPath();
            context.moveTo(tX(d.Borders[i][0].X), tY(d.Borders[i][0].Y));
            context.lineTo(tX(d.Borders[i][1].X), tY(d.Borders[i][1].Y));
            context.lineWidth = 1;
            context.strokeStyle = "black";
            context.stroke();
        }

        for (var i = 0; i < d.Snakes.length; i++) {
            context.beginPath();
            for (var n = 0; n < d.Snakes[i].Points.length - 1; n++) {
                context.moveTo(tX(d.Snakes[i].Points[n].X), tY(d.Snakes[i].Points[n].Y));
                context.lineTo(tX(d.Snakes[i].Points[n + 1].X), tY(d.Snakes[i].Points[n + 1].Y));
            }
            context.strokeStyle = d.Snakes[i].Color;
            context.lineWidth = GameSettings.SNAKE_RADIUS * 2;
            context.stroke();
        }

        var foodImg = new Image();
        foodImg.src = "/Content/img/food.png";
        for (var i = 0; i < d.Food.length; i++) {
            context.drawImage(foodImg, tX(d.Food[i].X - GameSettings.FOOD_RADIUS), tY(d.Food[i].Y - GameSettings.FOOD_RADIUS), GameSettings.FOOD_RADIUS * 2, GameSettings.FOOD_RADIUS * 2);
        }

        for (var i = 0; i < d.Shots.length; i++) {
            context.beginPath();
            context.arc(tX(d.Shots[i].X), tY(d.Shots[i].Y), GameSettings.SHOT_RADIUS, Math.PI * 2, false);
            context.fillStyle = "black";
            context.fill();
        }

        for (var i = 0; i < d.Ammo.length; i++) {
            context.beginPath();
            context.arc(tX(d.Ammo[i].X), tY(d.Ammo[i].Y), GameSettings.AMMO_RADIUS, Math.PI * 2, false);
            context.fillStyle = "black";
            context.fill();
        }

        for (var i = 0; i < d.Armor.length; i++) {
            context.beginPath();
            context.arc(tX(d.Armor[i].X), tY(d.Armor[i].Y), GameSettings.ARMOR_RADIUS, Math.PI * 2, false);
            context.fillStyle = "brown";
            context.fill();
        }

    } catch (err) {
        console.log(d);
        console.log(err);
    }
    count++;
    loopDone = true;
}

this.GameOver = function () {
    _this.Stop();
}

this.Stop = function () {
    clearInterval(_this.loopTimer);
    clearInterval(_this.fpsTimer);
    clearInterval(_this.dataTimer);
    window.removeEventListener("keydown", _this.KeyDown);
    window.removeEventListener("keyup", _this.KeyUp);
    callback();
}

window.addEventListener("keydown", keyDown)
window.addEventListener("keyup", keyUp)

var _leftKey = false;
var _rightKey = false;
var _boost = false;
var _break = false;
var _shoot = false;

function keyDown(data) {
    switch (data.keyCode) {
        case PlayerSettings.turnLeft:
            _leftKey = true;
            break;
        case PlayerSettings.turnRight:
            _rightKey = true;
            break;
        case PlayerSettings.shoot:
            _shoot = true;
            break;
        case PlayerSettings.boost:
            _boost = true;
            break;
        case PlayerSettings._break:
            _break = true;
            break;
    }
}

function keyUp(data) {
    switch (data.keyCode) {
        case PlayerSettings.turnLeft:
            _leftKey = false;
            break;
        case PlayerSettings.turnRight:
            _rightKey = false;
            break;
        case PlayerSettings.boost:
            _boost = false;
            break;
        case PlayerSettings._break:
            _break = false;
            break;
    }
}

init();

$(window).resize(resize);
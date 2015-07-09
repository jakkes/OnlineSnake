var canvas = document.getElementById("game");
var context = canvas.getContext("2d");

function connect(url, callback, force) {
    $.ajax({
        url: url,
        success: function (data) {
            callback(data.substring(0, data.indexOf("XKXKXK")));
        },
        timeout: 150,
        error: function(){
            if (force === true)
                connect(url, callback, force);
            console.log("Timeout");
            loopDone = true;
        }
    });
}

var loopTimer;
var leaderTimer;

var GameSettings;
var PlayerSettings;

PlayerSettings = {
    turnLeft: 37,
    turnRight: 39,
}

var _turningLeft = false;
function turnLeft() {
    if (!_turningLeft) {
        connect("/Game/TurnLeft", function (data) {
            if (data == "Success")
                _turningLeft = true;
        });
    }
}

var _turningRight = false;
function turnRight() {
    if (!_turningRight) {
        connect("/Game/TurnRight", function (data) {
            if (data == "Success")
                _turningRight = true;
        });
    }
}

function turnStop() {
    connect("/Game/TurnStop", function (data) {
        if (data == "Success") {
            _turningLeft = false;
            _turningRight = false;
        }
    });
}

function init() {
    connect("/Game/GetSettings", function (data) {
        GameSettings = JSON.parse(data);
        connect("/Game/GetAllTimeLeader", function(data){
            var d = JSON.parse(data);
            $("#AllTimeLeaderLb").text("All time leader: " + d.Name + " - " + d.Score + " points");
            connect("/Game/Join", function (data) {
                if (data == "Success") {
                    StartLoop();
                }
            }, true);
        }, true);
    }, true);
}

var count = 0;
var t = 0;
var time;

function StartLoop() {
    getLeader();
    leaderTimer = setInterval(getLeader, 2000);
    loopTimer = setInterval(loop, 50);
}

var loopDone = true;
function loop() {
    if (loopDone) {
        loopDone = false;
        time = new Date().getTime();
        connect("/Game/Loop", draw);
    }
}

function getLeader() {
    connect("/Game/GetLeader", function (data) {
        var d = JSON.parse(data);
        $("#LeaderLb").text("Current leader: " + d.Name + " - " + d.Score + " points");
    });
    connect("/Game/GetAllTimeLeader", function (data) {
        var d = JSON.parse(data);
        $("#AllTimeLeaderLb").text("All time leader: " + d.Name + " - " + d.Score + " points");
    });
};

function draw(data) {
    try {
        var d = JSON.parse(data);
        $("#ScoreLb").text("Score: " + d.Name + " - " + d.Score + " points");
        context.clearRect(0, 0, canvas.width, canvas.height);
        if (d.ConnectionCode != "200") {
            Stop();
            return;
        }

        for (var n = 0; n < d.Player.length; n++) {
            context.beginPath();
            for (var i = 0; i < d.Player[n].length - 1; i++) {
                context.moveTo(d.Player[n][i].X + canvas.clientWidth / 2, d.Player[n][i].Y + canvas.clientHeight / 2);
                context.lineTo(d.Player[n][i + 1].X + canvas.clientWidth / 2, d.Player[n][i + 1].Y + canvas.clientHeight / 2);
            }
            context.strokeStyle = "blue";
            context.lineWidth = GameSettings.SNAKE_RADIUS * 2;
            context.stroke();
        }

        for (var i = 0; i < d.Borders.length; i++) {
            context.beginPath();
            context.moveTo(d.Borders[i][0].X + canvas.clientWidth / 2, d.Borders[i][0].Y + canvas.clientHeight / 2);
            context.lineTo(d.Borders[i][1].X + canvas.clientWidth / 2, d.Borders[i][1].Y + canvas.clientHeight / 2);
            context.lineWidth = 1;
            context.strokeStyle = "black";
            context.stroke();
        }

        for (var i = 0; i < d.Opponents.length; i++) {
            context.beginPath();
            for (var n = 0; n < d.Opponents[i].length - 1; n++) {
                context.moveTo(d.Opponents[i][n].X + canvas.clientWidth / 2, d.Opponents[i][n + 1].Y + canvas.clientHeight / 2);
                context.lineTo(d.Opponents[i][n + 1].X + canvas.clientWidth / 2, d.Opponents[i][n + 1].Y + canvas.clientHeight / 2);
            }
            context.strokeStyle = "purple";
            context.lineWidth = GameSettings.SNAKE_RADIUS * 2;
            context.stroke();
        }

        for (var i = 0; i < d.Food.length; i++) {
            context.beginPath();
            context.arc(d.Food[i].X + canvas.clientWidth / 2, d.Food[i].Y + canvas.clientHeight / 2, GameSettings.FOOD_RADIUS, Math.PI * 2, false);
            context.fillStyle = "red";
            context.fill();
        }
    } catch (err) {
        console.log(data);
        console.log(err)
    }
    t += (new Date().getTime() - time);
    count++;

    if (count >= 40) {
        $("#LatencyLb").text("Latency: " + t / count + " ms");
        t = 0;
        count = 0;
    }

    loopDone = true;
}

function Stop() {
    clearInterval(loopTimer);
    clearInterval(leaderTimer);
}

window.addEventListener("keydown", keyDown)
window.addEventListener("keyup", keyUp)

function keyDown(data) {
    switch (data.keyCode) {
        case PlayerSettings.turnLeft:
            turnLeft();
            break;
        case PlayerSettings.turnRight:
            turnRight();
            break;
    }
}

function keyUp(data) {
    switch (data.keyCode) {
        case PlayerSettings.turnLeft:
        case PlayerSettings.turnRight:
            turnStop();
            break;
    }
}

init();
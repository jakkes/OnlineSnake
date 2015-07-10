var Game = function (uiData) {

    _this = this;

    this.canvas = uiData.canvas;
    this.context = this.canvas.getContext("2d");
    this.ammo = uiData.ammo;
    this.armor = uiData.armor;
    this.score = uiData.score;
    this.leader = uiData.leader;
    this.leaderboard = uiData.leaderboard;
    this.personal = uiData.personalHighscore;
    this.fps = uiData.fps;
    this.boost = uiData.boost;
    this.break = uiData.break;
    this.players = uiData.players;
    this.online = uiData.onlineCount;

    this.GameSettings;
    this.PlayerSettings = {
        turnLeft: 37,
        turnRight: 39,
        shoot: 32,
        boost: 16,
        timeout: 75,
        fps: 30
    };

    this.foodImg = new Image();
    this.foodImg.src = "/Content/img/food.png";
    
    this.isLooping = false;
    this.loopTimer;
    this.frameCount = 0;
    this.fpsTimer;
    this.dataTimer;

    this.isBoosting = false;
    this.isTurningLeft = false;
    this.isTurningRight = false;
    this.isShooting = false;

    this.GetData = function () {
        $.getJSON("/Game/GetData", "", function (data) {
            if (data.ConnectionCode == "200") {
                //Current leader
                _this.leader.text(data.CurrentLeader.Name + "(" + data.CurrentLeader.Score + ")");

                //Leaderboard
                _this.leaderboard.children().remove();
                for (var i = 0; i < data.Leaderboard.length; i++)
                    _this.leaderboard.append("<li>" + (i + 1) + ". " + data.Leaderboard[i].Name + " (" + data.Leaderboard[i].Score + ")</li>");

                //Players online
                _this.players.children().remove();
                for(var i = 0; i < data.Players.length;i++)
                    _this.players.append("<li style=\"color:" + data.Players[i].Color + ";\">" + data.Players[i].Name + "</li>")

                //Personal highscore
                _this.personal.text(data.PersonalHighscore);

                //Online count
                _this.online.text(data.OnlineCount);
            }
        });
    }

    this.Loop = function () {

        _this.isLooping = true;

        var turn = "None";
        if(_this.isTurningLeft) turn = "Left";
        else if(_this.isTurningRight) turn = "Right";

        $.ajax({
            url: "/Game/Loop",
            type: "post",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify({
                "Turn": turn,
                "Boost": _this.isBoosting,
                "Shoot": _this.isShooting
            }),
            timeout: _this.PlayerSettings.timeout,
            success: function (data) {
                _this.isLooping = false;
                _this.isShooting = false;
                _this.Draw(data);
            },
            error: function (err) {
                console.log("Opted out of loop");
                _this.isLooping = false;
            }
        });
    }

    this.Draw = function (data) {
        function tx(x) {
            return x + _this.canvas.width / 2;
        }
        function ty(y) {
            return y + _this.canvas.height / 2;
        }

        if (data.ConnectionCode == "404") {
            _this.GameOver();
            return;
        } else if (data.ConnectionCode != "200") {
            console.log(data);
            _this.Stop();
            return;
        }

        //Set text
        _this.score.text(data.Score);
        _this.ammo.text(data.AmmoCount);
        _this.armor.text(data.PlayerArmor);
        _this.boost.text(Math.round(data.BoostStored / 100));

        //Clear for redraw
        _this.context.clearRect(0, 0, _this.canvas.width, _this.canvas.height);

        //Draw borders
        for (var i = 0; i < data.Borders.length; i++) {
            _this.context.beginPath();
            _this.context.moveTo(tx(data.Borders[i][0].X), ty(data.Borders[i][0].Y));
            _this.context.lineTo(tx(data.Borders[i][1].X), ty(data.Borders[i][1].Y));
            _this.context.lineWidth = 1;
            _this.context.strokeStyle = "black";
            _this.context.stroke();
        }

        //Draw snakes
        for (var i = 0; i < data.Snakes.length; i++) {
            _this.context.beginPath();
            for (var n = 0; n < data.Snakes[i].Points.length - 1; n++) {
                _this.context.moveTo(tx(data.Snakes[i].Points[n].X), ty(data.Snakes[i].Points[n].Y));
                _this.context.lineTo(tx(data.Snakes[i].Points[n + 1].X), ty(data.Snakes[i].Points[n + 1].Y));
            }
            _this.context.strokeStyle = data.Snakes[i].Color;
            _this.context.lineWidth = _this.GameSettings.SNAKE_RADIUS * 2;
            _this.context.stroke();
        }

        //Draw food
        for (var i = 0; i < data.Food.length; i++)
            _this.context.drawImage(_this.foodImg, tx(data.Food[i].X - _this.GameSettings.FOOD_RADIUS), ty(data.Food[i].Y - _this.GameSettings.FOOD_RADIUS), _this.GameSettings.FOOD_RADIUS * 2, _this.GameSettings.FOOD_RADIUS * 2);
        
        //Shots
        for (var i = 0; i < data.Shots.length; i++) {
            _this.context.beginPath();
            _this.context.arc(tx(data.Shots[i].X), ty(data.Shots[i].Y), _this.GameSettings.SHOT_RADIUS, Math.PI * 2, false);
            _this.context.fillStyle = "black";
            _this.context.fill();
        }

        //Ammo
        for (var i = 0; i < data.Ammo.length; i++) {
            _this.context.beginPath();
            _this.context.arc(tx(data.Ammo[i].X), ty(data.Ammo[i].Y), _this.GameSettings.AMMO_RADIUS, Math.PI * 2, false);
            _this.context.fillStyle = "black";
            _this.context.fill();
        }

        //Armor
        for (var i = 0; i < data.Armor.length; i++) {
            _this.context.beginPath();
            _this.context.arc(tx(data.Armor[i].X), ty(data.Armor[i].Y), _this.GameSettings.ARMOR_RADIUS, Math.PI * 2, false);
            _this.context.fillStyle = "brown";
            _this.context.fill();
        }

        _this.frameCount++;

    }

    this.CalcFPS = function () {
        _this.fps.text(_this.frameCount);
        _this.frameCount = 0;
    }

    this.KeyDown = function (key) {
        switch (key.keyCode) {
            case _this.PlayerSettings.turnLeft:
                _this.isTurningLeft = true;
                break;
            case _this.PlayerSettings.turnRight:
                _this.isTurningRight = true;
                break;
            case _this.PlayerSettings.boost:
                _this.isBoosting = true;
                break;
            case _this.PlayerSettings.shoot:
                _this.isShooting = true;
                break;
        }
    }

    this.KeyUp = function (key) {
        switch (key.keyCode) {
            case _this.PlayerSettings.turnLeft:
                _this.isTurningLeft = false;
                break;
            case _this.PlayerSettings.turnRight:
                _this.isTurningRight = false;
                break;
            case _this.PlayerSettings.boost:
                _this.isBoosting = false;
                break;
        }
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
    }

    window.addEventListener("keydown", _this.KeyDown);
    window.addEventListener("keyup", _this.KeyUp);
    
    $.getJSON("/Game/GetSettings", "", function (data) {
        _this.GameSettings = data;
        $.getJSON("/Game/Join", "", function (data) {
            if (data.ConnectionCode == "200") {
                _this.loopTimer = setInterval(_this.Loop, 1000 / _this.PlayerSettings.fps);
                _this.fpsTimer = setInterval(_this.CalcFPS, 1000);
                _this.dataTimer = setInterval(_this.GetData, 2000);
            }
        });
    });
}
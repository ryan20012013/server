var ip = location.host;
var hostUrl = "http://" + ip;
var apiUrl = hostUrl + "/api";

$("#serverOnOff").click(function(){
    $.post(apiUrl + "/shutdown", function(data, status){
        $("#result").text("Status: " + status + " | Is server Running: " + (data.ServerRunning ? "Yes" : "No"));
    });
});

$("#browse").click(function(){
    $.post(apiUrl + "/browse",
        {
            url : "https://music.youtube.com/watch?list=RDAMVMXyGTTtG1p6I"
        }, 
        function(data, status){
            $("#result").text("Status: " + status + " | website opened: " + data.url);
    });
});


$("#setVolume").click(function(){
    console.log("It clicked");
    $.post(apiUrl + "/audio",
        {
            volume : $("#percentageList").val() * 10,
            mute: $("#mute").is(":checked")
        }, 
        function(data, status){
            $("#result").text("Status: " + status + " | website opened: " + data.url);
    });
});

$(document).ready(function() {
    console.log("document ready");
    $.getJSON(hostUrl + "/ApiInfo.json", function(data) {
        console.log("methods: " + data.methods + " apis: " + data.apis);
        $($.parseJSON(JSON.stringify(data.methods))).each(function() {
            var option = $("<option />");
            option.html(this.label);
            option.val(this.value);
            $("#methodList").append(option);
        })
        $($.parseJSON(JSON.stringify(data.apis))).each(function() {
            var option = $("<option />");
            option.html(this.label);
            option.val(this.value);
            $("#apiList").append(option);
        })
    })
});
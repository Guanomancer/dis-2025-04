// Ref: https://docs.ml5js.org/#/reference/handpose

var cout = true;
var coutOnce = true;
var storeFrames = -1;

let handPose;
let video;
let hands = [];

let canvasWidth = 800;
let canvasHeight = 600;

let minAcceptableConfidence = 0.9;

let deviceId = "e64447b2-5aae-473d-a54f-230ad3dfe8b1";
let frameIdx = 0;

function apiPost(url, frameJson) {
    timeMs = Date.now();
    fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(frameJson)
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return true;
        })
        .then(data => {
            console.log(data);
        })
        .catch(error => {
            console.error('Error:', error);
        });
}

function preload() {
    handPose = ml5.handPose();
}

function setup() {
    createCanvas(canvasWidth, canvasHeight);

    video = createCapture(VIDEO);
    video.size(canvasWidth, canvasHeight);
    video.hide();

    handPose.detectStart(video, detectCallback);
}

function startDetect() {
    console.log("Preloading..");
    preload();
    setTimeout(() => {
        console.log("Detecting..");
        setup();
    }, 2000);
}

function detectCallback(results) {
    hands = results;
}

function draw() {
    image(video, 0, 0, canvasWidth, canvasHeight);

    for (let i = 0; i < hands.length; i++) {
        let hand = hands[i];
        let lowConfidence = hand.confidence < minAcceptableConfidence;
        if (cout) console.log("Hand: " + i);
        for (let j = 0; j < hand.keypoints.length; j++) {
            let keypoint = hand.keypoints[j];
            if (j == 0) {
                fill(0, 0, 0);
            } else if (lowConfidence) {
                fill(255, 0, 0);
            } else {
                digit = 1 + Math.floor((j - 1) / 4);
                index = 1 + (j - 1) % 4;
                var saturation = Math.floor(255 * index / 4);
                r = (digit & 1) ? saturation : 0;
                g = (digit & 2) ? saturation : 0;
                b = (digit & 4) ? saturation : 0;
                if (cout) console.log("D: " + digit + ", I: " + index + ", S: " + saturation, " C: " + r + ", " + g + ", " + b + "\n" + JSON.stringify(hand, null, 2));
                fill(r, g, b);
            }
            noStroke();
            circle(keypoint.x, keypoint.y, 20);
        }
    }

    if (hands.length > 0 && storeFrames != 0) {
        const url = "https://localhost:7002/api/Store/" + deviceId + "/" + frameIdx++;
        if (cout) console.log("Storing at: " + url + "\n" + hands);
        storeFrames = Math.max(storeFrames - 1, -1);
        apiPost(url, hands);
    }

    if (hands.length > 0 && cout && coutOnce) {
        coutOnce = false;
        cout = false;
    }
}

package nl.inholland.quizgame.model;

import com.fasterxml.jackson.annotation.JsonProperty;

public class QuizResult {

    @JsonProperty("playerName")
    private String playerName;

    @JsonProperty("totalQuestions")
    private int totalQuestions;

    @JsonProperty("correctQuestions")
    private int correctQuestions;

    @JsonProperty("date")
    private String date;

    public QuizResult() {}

    public QuizResult(String playerName, int totalQuestions, int correctQuestions, String date) {
        this.playerName = playerName;
        this.totalQuestions = totalQuestions;
        this.correctQuestions = correctQuestions;
        this.date = date;
    }

    public String getPlayerName() { return playerName; }
    public void setPlayerName(String playerName) { this.playerName = playerName; }
    public int getTotalQuestions() { return totalQuestions; }
    public void setTotalQuestions(int totalQuestions) { this.totalQuestions = totalQuestions; }
    public int getCorrectQuestions() { return correctQuestions; }
    public void setCorrectQuestions(int correctQuestions) { this.correctQuestions = correctQuestions; }
    public String getDate() { return date; }
    public void setDate(String date) { this.date = date; }
}

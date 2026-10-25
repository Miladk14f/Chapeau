package nl.inholland.quizgame.model;

import com.fasterxml.jackson.annotation.JsonProperty;

import java.util.ArrayList;
import java.util.List;

public class QuizResultFile {

    @JsonProperty("quizId")
    private String quizId;

    @JsonProperty("name")
    private String name;

    @JsonProperty("results")
    private List<QuizResult> results;

    public QuizResultFile() {
        this.results = new ArrayList<>();
    }

    public QuizResultFile(String quizId, String name) {
        this.quizId = quizId;
        this.name = name;
        this.results = new ArrayList<>();
    }

    public String getQuizId() { return quizId; }
    public void setQuizId(String quizId) { this.quizId = quizId; }
    public String getName() { return name; }
    public void setName(String name) { this.name = name; }
    public List<QuizResult> getResults() { return results; }
    public void setResults(List<QuizResult> results) { this.results = results; }
}

package nl.inholland.quizgame.controller;

import javafx.fxml.FXML;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.scene.control.Alert;
import javafx.scene.control.Label;
import javafx.stage.Stage;
import nl.inholland.quizgame.manager.GameManager;
import nl.inholland.quizgame.model.QuizResult;
import nl.inholland.quizgame.service.ResultsSaver;
import nl.inholland.quizgame.state.MenuState;

import java.io.IOException;
import java.time.Instant;

public class ResultsController {

    @FXML private Label playerNameLabel;
    @FXML private Label scoreLabel;
    @FXML private Label totalLabel;
    @FXML private Label percentLabel;
    @FXML private Label savedLabel;

    private final GameManager gameManager = GameManager.getInstance();

    @FXML
    public void initialize() {
        int score = gameManager.getScore();
        int total = gameManager.getCurrentQuiz() != null
                ? gameManager.getCurrentQuiz().getQuestionCount() : 0;
        int percent = total > 0 ? (score * 100 / total) : 0;

        playerNameLabel.setText("Player: " + gameManager.getPlayerName());
        scoreLabel.setText(String.valueOf(score));
        totalLabel.setText("out of " + total);
        percentLabel.setText(percent + "%");

        saveResult(score, total);
    }

    private void saveResult(int score, int total) {
        if (gameManager.getCurrentQuiz() == null) return;

        QuizResult result = new QuizResult(
                gameManager.getPlayerName(),
                total,
                score,
                Instant.now().toString()
        );

        try {
            ResultsSaver.save(gameManager.getCurrentQuiz(), result);
            savedLabel.setText("Result saved to " + gameManager.getCurrentQuiz().getQuizId() + "-results.json");
        } catch (IOException e) {
            savedLabel.setText("Could not save result: " + e.getMessage());
            savedLabel.getStyleClass().add("error-label");
        }
    }

    @FXML
    private void onBackToMenu() {
        gameManager.setState(new MenuState());
        gameManager.setCurrentQuiz(null);
        try {
            Parent menuRoot = FXMLLoader.load(
                    getClass().getResource("/nl/inholland/quizgame/menu-view.fxml"));
            Stage stage = (Stage) playerNameLabel.getScene().getWindow();
            stage.setScene(new Scene(menuRoot, 800, 600));
        } catch (IOException e) {
            Alert alert = new Alert(Alert.AlertType.ERROR);
            alert.setTitle("Navigation error");
            alert.setContentText("Could not return to menu: " + e.getMessage());
            alert.showAndWait();
        }
    }
}

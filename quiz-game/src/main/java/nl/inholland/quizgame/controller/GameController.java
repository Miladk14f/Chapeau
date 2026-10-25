package nl.inholland.quizgame.controller;

import javafx.animation.KeyFrame;
import javafx.animation.Timeline;
import javafx.beans.binding.Bindings;
import javafx.fxml.FXML;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.scene.control.*;
import javafx.scene.layout.VBox;
import javafx.stage.Modality;
import javafx.stage.Stage;
import javafx.util.Duration;
import nl.inholland.quizgame.manager.GameManager;
import nl.inholland.quizgame.model.Question;
import nl.inholland.quizgame.model.UnknownQuestion;
import nl.inholland.quizgame.state.ResultsState;

import java.io.IOException;
import java.util.List;

public class GameController {

    @FXML private Label questionNumberLabel;
    @FXML private Label questionTitleLabel;
    @FXML private VBox choicesBox;
    @FXML private ProgressBar timerBar;
    @FXML private Label timerLabel;
    @FXML private Label scoreLabel;
    @FXML private Button submitButton;
    @FXML private Label playerNameLabel;

    private final GameManager gameManager = GameManager.getInstance();
    private Timeline countdownTimer;
    private int totalSeconds;
    private int secondsLeft;
    private ToggleGroup toggleGroup;

    @FXML
    public void initialize() {
        // Observer pattern: scoreLabel updates automatically when scoreProperty changes
        scoreLabel.textProperty().bind(
                Bindings.concat("Score: ", gameManager.scoreProperty().asString()));

        playerNameLabel.setText("Player: " + gameManager.getPlayerName());
        showCurrentQuestion();
    }

    private void showCurrentQuestion() {
        Question question = getCurrentQuestion();
        if (question == null) {
            finishQuiz();
            return;
        }

        int index = gameManager.getCurrentQuestionIndex();
        int total = gameManager.getCurrentQuiz().getQuestionCount();
        questionNumberLabel.setText("Question " + (index + 1) + " of " + total);
        questionTitleLabel.setText(question.getTitle());

        choicesBox.getChildren().clear();
        toggleGroup = new ToggleGroup();

        if (question instanceof UnknownQuestion uq) {
            Label unsupported = new Label("(Question type '" + uq.getOriginalType() + "' is not supported — skipped)");
            unsupported.getStyleClass().add("unsupported-label");
            choicesBox.getChildren().add(unsupported);
            submitButton.setText("Skip");
        } else {
            List<String> choices = question.getChoices();
            for (String choice : choices) {
                RadioButton rb = new RadioButton(choice);
                rb.setToggleGroup(toggleGroup);
                rb.getStyleClass().add("choice-button");
                choicesBox.getChildren().add(rb);
            }
            submitButton.setText((index < total - 1) ? "Submit & Next" : "Finish Quiz");
        }

        startTimer(question.getTimeLimit());
    }

    private void startTimer(int seconds) {
        if (countdownTimer != null) {
            countdownTimer.stop();
        }
        totalSeconds = seconds;
        secondsLeft = seconds;
        updateTimerDisplay();

        countdownTimer = new Timeline(new KeyFrame(Duration.seconds(1), e -> {
            secondsLeft--;
            updateTimerDisplay();
            if (secondsLeft <= 0) {
                countdownTimer.stop();
                onTimeUp();
            }
        }));
        countdownTimer.setCycleCount(seconds);
        countdownTimer.play();
    }

    private void updateTimerDisplay() {
        timerLabel.setText(secondsLeft + "s");
        double progress = (double) secondsLeft / totalSeconds;
        timerBar.setProgress(progress);
        if (progress < 0.25) {
            timerBar.getStyleClass().removeAll("timer-normal", "timer-warning");
            if (!timerBar.getStyleClass().contains("timer-critical")) {
                timerBar.getStyleClass().add("timer-critical");
            }
        } else if (progress < 0.5) {
            timerBar.getStyleClass().removeAll("timer-normal", "timer-critical");
            if (!timerBar.getStyleClass().contains("timer-warning")) {
                timerBar.getStyleClass().add("timer-warning");
            }
        } else {
            timerBar.getStyleClass().removeAll("timer-warning", "timer-critical");
            if (!timerBar.getStyleClass().contains("timer-normal")) {
                timerBar.getStyleClass().add("timer-normal");
            }
        }
    }

    private void onTimeUp() {
        // Time expired: no answer counted, move on
        submitButton.setDisable(true);
        showTimedOutFeedback();
    }

    private void showTimedOutFeedback() {
        Label feedback = new Label("Time's up! Moving to next question...");
        feedback.getStyleClass().add("feedback-wrong");
        choicesBox.getChildren().add(feedback);

        Timeline pause = new Timeline(new KeyFrame(Duration.seconds(1.5), e -> advanceOrFinish()));
        pause.play();
    }

    @FXML
    private void onSubmit() {
        if (countdownTimer != null) {
            countdownTimer.stop();
        }

        Question question = getCurrentQuestion();
        if (question instanceof UnknownQuestion) {
            advanceOrFinish();
            return;
        }

        Toggle selected = toggleGroup.getSelectedToggle();
        if (selected == null) {
            showInlineError("Please select an answer before submitting.");
            if (countdownTimer != null) countdownTimer.play();
            return;
        }

        String answer = ((RadioButton) selected).getText();
        boolean correct = question.evaluate(answer);

        if (correct) {
            gameManager.incrementScore(); // Observer: scoreLabel updates automatically
        }

        showAnswerFeedback(correct);
    }

    private void showAnswerFeedback(boolean correct) {
        submitButton.setDisable(true);
        Label feedback = new Label(correct ? "Correct!" : "Incorrect.");
        feedback.getStyleClass().add(correct ? "feedback-correct" : "feedback-wrong");
        choicesBox.getChildren().add(feedback);

        Timeline pause = new Timeline(new KeyFrame(Duration.seconds(1.0), e -> advanceOrFinish()));
        pause.play();
    }

    private void advanceOrFinish() {
        submitButton.setDisable(false);
        if (gameManager.hasNextQuestion()) {
            gameManager.nextQuestion();
            showCurrentQuestion();
        } else {
            gameManager.nextQuestion(); // move past last
            finishQuiz();
        }
    }

    private void finishQuiz() {
        if (countdownTimer != null) countdownTimer.stop();
        gameManager.setState(new ResultsState());
        try {
            Parent resultsRoot = FXMLLoader.load(
                    getClass().getResource("/nl/inholland/quizgame/results-view.fxml"));
            Stage stage = (Stage) submitButton.getScene().getWindow();
            stage.setScene(new Scene(resultsRoot, 800, 600));
        } catch (IOException e) {
            showAlert("Navigation error", "Could not open results screen: " + e.getMessage());
        }
    }

    private void showInlineError(String message) {
        Alert alert = new Alert(Alert.AlertType.WARNING);
        alert.setTitle("No answer selected");
        alert.setHeaderText(null);
        alert.setContentText(message);
        alert.initModality(Modality.APPLICATION_MODAL);
        alert.showAndWait();
    }

    private void showAlert(String title, String message) {
        Alert alert = new Alert(Alert.AlertType.ERROR);
        alert.setTitle(title);
        alert.setHeaderText(title);
        alert.setContentText(message);
        alert.showAndWait();
    }

    private Question getCurrentQuestion() {
        GameManager gm = gameManager;
        if (gm.getCurrentQuiz() == null) return null;
        int index = gm.getCurrentQuestionIndex();
        List<Question> questions = gm.getCurrentQuiz().getQuestions();
        if (index >= questions.size()) return null;
        return questions.get(index);
    }
}

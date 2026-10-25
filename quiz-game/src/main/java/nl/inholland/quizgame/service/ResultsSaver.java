package nl.inholland.quizgame.service;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.SerializationFeature;
import nl.inholland.quizgame.model.Quiz;
import nl.inholland.quizgame.model.QuizResult;
import nl.inholland.quizgame.model.QuizResultFile;

import java.io.File;
import java.io.IOException;

public class ResultsSaver {

    private static final ObjectMapper MAPPER = new ObjectMapper()
            .enable(SerializationFeature.INDENT_OUTPUT);

    private ResultsSaver() {}

    public static void save(Quiz quiz, QuizResult result) throws IOException {
        String fileName = quiz.getQuizId() + "-results.json";
        File file = new File(fileName);

        QuizResultFile resultFile = loadExisting(file, quiz);
        resultFile.getResults().add(result);

        MAPPER.writeValue(file, resultFile);
    }

    private static QuizResultFile loadExisting(File file, Quiz quiz) {
        if (!file.exists()) {
            return new QuizResultFile(quiz.getQuizId(), quiz.getTitle());
        }
        try {
            QuizResultFile existing = MAPPER.readValue(file, QuizResultFile.class);
            if (existing.getResults() == null) {
                existing.setResults(new java.util.ArrayList<>());
            }
            return existing;
        } catch (Exception e) {
            // corrupt or unreadable file — start fresh rather than crashing
            return new QuizResultFile(quiz.getQuizId(), quiz.getTitle());
        }
    }
}

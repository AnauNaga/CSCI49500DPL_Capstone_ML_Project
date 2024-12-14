import pandas as pd
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.neighbors import KNeighborsClassifier
from sklearn.metrics import classification_report, accuracy_score
import nltk
from nltk.corpus import stopwords

# Download NLTK stopwords if not already downloaded
nltk.download('stopwords')
stop_words = set(stopwords.words('english'))

# Function to preprocess text data (removing stop words)
def preprocess_text(text):
    return ' '.join(word for word in text.split() if word.lower() not in stop_words)

# Load training dataset
train_file = 'concatKNN.xls'
train_data = pd.read_excel(train_file, sheet_name=0, header=0, names=['Category', 'Text'])
train_data['Text'] = train_data['Text'].apply(preprocess_text)

# Load testing dataset
test_file = 'Training2.xls'
test_data = pd.read_excel(test_file, sheet_name=0, header=0, names=['Category', 'Text'])
test_data['Text'] = test_data['Text'].apply(preprocess_text)

# Vectorize the text data using TF-IDF
tfidf = TfidfVectorizer(max_features=5000)
X_train = tfidf.fit_transform(train_data['Text'])
Y_train = train_data['Category']

X_test = tfidf.transform(test_data['Text'])  # Transform test data using the same vectorizer
Y_test = test_data['Category']

# Train the KNN model on the original training dataset
knn = KNeighborsClassifier(k=5)
knn.fit(X_train, Y_train)

# Predict on the test dataset
test_predictions = knn.predict(X_test)

# Evaluate the initial model on the test dataset
test_accuracy_initial = accuracy_score(Y_test, test_predictions)
print(f"Initial Testing Accuracy: {test_accuracy_initial:.4f}\n")
print("Initial Testing Classification Report:\n")
print(classification_report(Y_test, test_predictions))

# Add correctly classified test samples to the training dataset
correctly_classified_indices = (test_predictions == Y_test).to_numpy().nonzero()[0]
new_train_data = test_data.iloc[correctly_classified_indices]

# Extend the training dataset
extended_train_data = pd.concat([train_data, new_train_data], ignore_index=True)

# Re-vectorize the extended training data
X_extended_train = tfidf.fit_transform(extended_train_data['Text'])
Y_extended_train = extended_train_data['Category']

# Retrain the KNN model on the extended training dataset
knn.fit(X_extended_train, Y_extended_train)

# Reevaluate the model on both the training and testing datasets
# Reevaluate on the test dataset
X_test_extended = tfidf.transform(test_data['Text'])
final_test_predictions = knn.predict(X_test_extended)
test_accuracy_final = accuracy_score(Y_test, final_test_predictions)
print(f"Final Testing Accuracy: {test_accuracy_final:.4f}\n")
print("Final Testing Classification Report:\n")
print(classification_report(Y_test, final_test_predictions))

# Evaluate the final model on the extended training dataset
final_train_predictions = knn.predict(X_extended_train)
train_accuracy_final = accuracy_score(Y_extended_train, final_train_predictions)
print(f"Final Training Accuracy: {train_accuracy_final:.4f}\n")
print("Final Training Classification Report:\n")
print(classification_report(Y_extended_train, final_train_predictions))

# Save results to files
train_output_file = 'final_training_result.xlsx'
train_output_data = pd.DataFrame({
    'Text': extended_train_data['Text'],
    'Actual Category': Y_extended_train,
    'Predicted Category': final_train_predictions
})
train_output_data.to_excel(train_output_file, index=False)
print(f"Final training results saved to {train_output_file}")

test_output_file = 'final_testing_result.xlsx'
test_output_data = pd.DataFrame({
    'Text': test_data['Text'],
    'Actual Category': Y_test,
    'Predicted Category': final_test_predictions
})
test_output_data.to_excel(test_output_file, index=False)
print(f"Final testing results saved to {test_output_file}")


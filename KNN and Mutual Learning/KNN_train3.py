import pandas as pd
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.neighbors import KNeighborsClassifier
from sklearn.metrics import classification_report, accuracy_score
import nltk
from nltk.corpus import stopwords

# Download NLTK stopwords if not already downloaded
nltk.download('stopwords')
stop_words = set(stopwords.words('english'))

# Load data from Excel file
file_path = 'Training3.xls'
data = pd.read_excel(file_path, sheet_name=0, header=0, names=['Category', 'Text'])

# Preprocess text (remove stop words)
data['Text'] = data['Text'].apply(lambda x: ' '.join(
    word for word in x.split() if word.lower() not in stop_words))

# Vectorize the text data using TF-IDF
tfidf = TfidfVectorizer(max_features=5000)  # Limit to top 5000 words to reduce dimensionality
X = tfidf.fit_transform(data['Text'])
y = data['Category']

# Train a K-Nearest Neighbors classifier on the entire dataset
knn = KNeighborsClassifier(n_neighbors=5)  # k=5, can be adjusted
knn.fit(X, Y)

# Make predictions on the entire dataset
predictions = knn.predict(X)

# Calculate and display accuracy and classification report
accuracy = accuracy_score(Y, predictions)
print(f'Accuracy: {accuracy:.4f}\n')
print("Classification Report:\n")
print(classification_report(Y, predictions, target_names=data['Category'].unique()))

# Prepare DataFrame with predictions for output
output_data = pd.DataFrame({
    'Text': data['Text'],
    'Actual Category': data['Category'],
    'Predicted Category': predictions
})

# Save the output data to an Excel file
output_file = 'result_ML.xlsx'
output_data.to_excel(output_file, index=False)
print(f"Predictions saved to {output_file}")

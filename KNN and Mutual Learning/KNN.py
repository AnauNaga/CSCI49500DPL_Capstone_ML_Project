import pandas as pd
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.model_selection import train_test_split
from sklearn.neighbors import KNeighborsClassifier
from sklearn.metrics import classification_report, accuracy_score
import nltk
from nltk.corpus import stopwords

# Download NLTK stopwords if not already downloaded
nltk.download('stopwords')
stop_words = set(stopwords.words('english'))

# Load data from Excel file
file_path = 'Stem-Lemma.xls'
data = pd.read_excel(file_path, sheet_name=0, header=0, names=['Category', 'Text'])

# Preprocess text (basic preprocessing, removing stop words)
data['Text'] = data['Text'].apply(lambda x: ' '.join(
    word for word in x.split() if word.lower() not in stop_words))

# Vectorize the text data using TF-IDF
tfidf = TfidfVectorizer(max_features=5000)  # Limit to top 5000 words to reduce dimensionality
X = tfidf.fit_transform(data['Text'])
y = data['Category']

# Split data into train and test sets
X_train, X_test, Y_train, Y_test = train_test_split(X, Y, test_size=0.9999, random_state=50)

# Train a K-Nearest Neighbors classifier
knn = KNeighborsClassifier(k=5)  # k=5, can be adjusted
knn.fit(X_train, Y_train)

# Make predictions on the test set
Y_pred = knn.predict(X_test)

# Evaluate the classifier
accuracy = accuracy_score(Y_test, Y_pred)
print(f'\nAccuracy: {accuracy:.4f}')
print("\nClassification Report:\n")
print(classification_report(Y_test, Y_pred, target_names=data['Category'].unique()))

# Prepare DataFrame with predictions for output
output_data = pd.DataFrame({
    'Text': data['Text'].iloc[Y_test.index],  # Get the actual text from the test set
    'Actual Category': Y_test.values,
    'Predicted Category': Y_pred
})

# Save the output data to an Excel file
output_file = 'training3_result_classification.xlsx'
output_data.to_excel(output_file, index=False)
print(f"Predictions saved to {output_file}")


#Imports
from sklearn import svm
from sklearn import datasets
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.metrics import classification_report
import pandas as pd
import csv

#Load training and test data
data = pd.read_csv('BBC_train_full.csv')
test = pd.read_csv('test_data.csv')
test_labels = pd.read_csv('test_labels.csv')

stem_data = pd.read_csv('Stem-Lemma.txt')
train2_data = pd.read_csv('Training2.txt')

vectorizer = TfidfVectorizer()

X_train = train2_data.drop('category', axis=1).values 
Y_train = train2_data['category'].values
X_test = test.to_numpy()
Y_test = test_labels.to_numpy()

#data processing
X =[]
for x in X_train:
    X.append(x[0])

XtestArr = []
for x in X_test:
    XtestArr.append(x[0])

YtestArr = []
for x in Y_test:
    YtestArr.append(x[0])
Y_test = YtestArr

X_train = vectorizer.fit_transform(X)
X_test = vectorizer.transform(XtestArr)

#Train model
clf = svm.SVC(kernel = "sigmoid")
clf.fit(X_train,Y_train)

pred = clf.predict(X_test)

#Calculate accuracy
accuracy = clf.score(X_test,Y_test)
print("Accuracy: ", accuracy)

#Read training3
train3 = pd.read_csv('Training3.txt')
text = train3['text'].values
labels = train3['category'].values

#Vectorize
X3 = vectorizer.transform(text)

#Print predictions of training3 as test set
pred_3 = clf.predict(X3)
for pred in pred_3:
    print(f"'{pred}',", end="")
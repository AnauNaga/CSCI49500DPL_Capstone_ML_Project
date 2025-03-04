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
train1_data = pd.read_csv('Training1.txt')

vectorizer = TfidfVectorizer()

X_train = train1_data.drop('category', axis=1).values 
Y_train = train1_data['category'].values
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
print(pred)

#Calculate accuracy
accuracy = clf.score(X_test,Y_test)
print("Accuracy: ", accuracy)

print(classification_report(pred, Y_test))

#Read training3
train3 = pd.read_csv('Training3.txt')
text = train3['text'].values
labels = train3['category'].values

#Vectorize
X3 = vectorizer.transform(text)

#Print predictions of training3 as test set
pred_3 = clf.predict(X3)
print(pred_3)

#Concatenation function
def concatLabels(outpred):
    concat_labels = []
    for i in range(len(pred_3)):
        if pred_3[i] == labels[i]:
            concat_labels.append(pred_3[i])
        elif outpred[i] == labels[i]:
            concat_labels.append(outpred[i])
        else:
            concat_labels.append(pred_3[i])
    return concat_labels

#Get predicted labels from other ML models
SVMLabels = ['business','entertainment','sport','politics','politics','sport','politics','business','tech','entertainment','sport','sport','tech','business','sport','entertainment','tech','sport','politics','sport','business','business','business','politics','entertainment','sport','politics','tech','business','business','tech','entertainment','entertainment','tech','tech','sport','sport','entertainment','tech','entertainment','sport','business','tech','business','sport','entertainment','business','business','entertainment','sport','business','sport','tech','politics','politics','tech','business','politics','entertainment','business','business','politics','sport','tech','sport','sport','sport','politics','business','sport','business','tech','sport','business','business','sport','politics','tech','sport','business','tech','sport','sport','business','business','business','business','politics','entertainment','tech','business','business','sport','business','entertainment','business','entertainment','sport','politics','business','business','tech','entertainment','business','business','business','politics','business','politics','tech','entertainment','politics','entertainment','sport','tech','politics','business','politics','sport','business','business','entertainment','tech','business','business','sport','sport','entertainment','sport','politics','sport','business','sport','business','politics','politics','business','politics','entertainment','entertainment','sport','business','entertainment','sport','politics','politics','tech','entertainment','business','politics','business','sport','tech','business','sport','entertainment','sport','entertainment','sport','business','sport','politics','entertainment','tech','business','sport','politics','tech','sport','entertainment','business','sport','entertainment','sport','tech','sport','tech','politics','sport','business','tech','tech','tech','politics','tech','entertainment','tech','sport','business','tech','business','entertainment','sport','tech','politics','politics','politics','politics','entertainment','entertainment','business','business','business','politics','sport','sport','tech','sport','politics','business','business','tech','politics','tech','business','tech','politics','politics','sport','business','politics','sport','business','tech','business','business','entertainment','entertainment','sport','politics','tech','tech','politics','business','business','sport','entertainment','entertainment','politics','politics','business','business','tech','tech','sport','sport','politics','tech','sport','entertainment','sport','sport','politics','entertainment','tech','sport','entertainment','business','politics','sport','business','tech','entertainment','sport','politics','business','entertainment','politics','entertainment','politics','sport','politics','business','business','politics','business','tech','sport','business','entertainment','business','sport','sport','entertainment','tech','politics','tech','politics','politics','sport','sport','sport','business','tech','entertainment','business','sport','business','sport','business','tech','business','business','business','business','sport','sport','sport','business','business','entertainment','business','business','business','sport','sport','sport','tech','business','sport','sport','sport','business','entertainment','tech','tech','tech','politics','business','business','tech','business','tech','sport','sport','politics','politics','politics','sport','sport','politics','politics','tech','business','sport','sport','sport','sport','business','sport','business','business','politics','business','sport','politics','sport','tech','tech','sport','tech','business','entertainment','business','sport','business','tech','tech','politics','entertainment','business','entertainment','business','tech','politics','entertainment','politics','entertainment','entertainment','tech','business','tech','tech','business','politics','sport','entertainment','business','politics','entertainment','business','tech','sport','tech','politics','sport','politics','business','politics','politics','business','business','business','tech','politics','business','politics','business','tech','politics','sport','politics','politics','sport','entertainment','tech','tech','business','tech','sport','business','business','sport','sport','tech','tech','tech','sport','entertainment','sport','politics','business','politics','politics','entertainment','tech','business','tech','sport','entertainment','entertainment','sport','business','entertainment','tech','tech','sport','entertainment','business','politics','tech','politics','tech','tech','business','politics','business','tech','politics','entertainment','sport','tech','business','tech','entertainment','tech','tech','sport','politics','sport','sport','politics','entertainment','entertainment','business','politics','politics','sport','business','business','tech','business','tech','tech','sport','business','politics','sport','business','business','sport','tech','sport','tech','entertainment','sport','sport','entertainment','business','tech','sport','sport','politics','tech','business','politics','sport','sport','politics','business','tech','tech','tech','business','sport','entertainment','entertainment','tech','business','politics','tech','sport','sport','tech','tech','politics','business','sport','entertainment','entertainment','sport','entertainment','tech','tech','business','business','business','sport','sport','tech','sport','business','tech','business','business','sport','business','politics','business','business','entertainment','politics','tech','politics','business','entertainment','politics','tech','entertainment','sport','politics','entertainment','business','tech','business','tech','entertainment','tech','sport','business','business','politics','entertainment','politics']

knnres = pd.read_csv('result.csv')
knnLabels = knnres['PredictedCategory'].values
concatSVM = concatLabels(SVMLabels)

concatKNN = concatLabels(knnLabels)

KNNfile = []
for i in range(len(text)):
    KNNfile.append(f"{concatKNN[i]}, {text[i]}")

MLXtrain = X3
MLYtrain = concatSVM
clf.fit(MLXtrain, MLYtrain)

MLpred = clf.predict(X_test)

#Calculate accuracy
MLaccuracy = clf.score(X_test,Y_test)
print("Accuracy: ", MLaccuracy)
print("Mutual Learning improved accuracy by ", (MLaccuracy-accuracy))

print(classification_report(MLpred, Y_test))
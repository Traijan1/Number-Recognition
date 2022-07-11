from PIL import Image, ImageOps
import numpy as np
from tensorflow import keras

import matplotlib.pyplot as plt

model = keras.models.load_model("model.h5")

img = Image.open("img.jpg")
img = img.resize((28, 28), resample=Image.BICUBIC)
img = ImageOps.grayscale(img)

data = np.asarray(img).astype(np.float32) / 255.

data = data.reshape(1, 28, 28, 1)

pred = model.predict(data)

pred_sort = np.sort(pred[0])

for x in range(pred_sort.size):
    if pred[0][x] == pred_sort[pred_sort.size - 1]:
        print(x)


# %%
# import pandas module
import pandas as pd
import matplotlib
 
# read the data
df = pd.read_csv("../_dummyCTFTrajectories.csv")
df.columns

# %%
df["Time"] = pd.to_timedelta(df["Time"],'s')
df = df.resample("100ms",on="Time").mean()
for heading in df.columns:
    if "Position" in heading:
        df["Velocity" + heading[-2:]] = df[heading].diff()
        df["Acceleration" + heading[-2:]] = df[heading].diff().diff()
i = 0
while True:
    


# %%
ax = df[["PositionX0","PositionY0"]].plot(rot=90)

# %%
c = df_v["PositionX0"].rolling("5s").corr(df_v["PositionY0"])
c



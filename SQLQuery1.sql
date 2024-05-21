create TABLE catagory(
c_id INT identity(1,1) NOT NULL PRIMARY KEY,
m_cId INT REFERENCES catagory(C_id), 
name VARCHAR(20) NOT NULL
);
drop table catagory
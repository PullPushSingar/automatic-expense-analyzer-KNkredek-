import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { Line } from 'react-chartjs-2';
import { Container, Grid,Box, List, ListItem, ListItemText, Paper, Typography } from '@mui/material';
import { startOfWeek, startOfMonth,} from 'date-fns';
import 'chart.js/auto';


const App = () => {
  const [operations, setOperations] = useState([]);

  const [stats, setStats] = useState({
    totalSpent: 0,
    totalEarned: 0,
    averageTransaction: 0,
    largestTransaction: 0,
    totalTransactions: 0
  });


  const [dailyChartData, setDailyChartData] = useState({
    labels: [],
    datasets: [{
      label: 'Saldo po operacji (tydzień)',
      data: [],
     
    }],
  });
  const [weeklyChartData, setWeeklyChartData] = useState({
    labels: [],
    datasets: [{
      label: 'Saldo po operacji (tydzień)',
      data: [],
      
    }],
  });
  const [monthlyChartData, setMonthlyChartData] = useState({
    labels: [],
    datasets: [{
      label: 'Saldo po operacji (miesiąc)',
      data: [],
      
    }],
  });


  const groupByDay = (operations) => {
    const days = {};
  
    operations.forEach((operation) => {
      const date = new Date(operation.operationDate);
      const key = date.toISOString().split('T')[0]; 
  
      if (!days[key]) {
        days[key] = {
          lastOperationAmount: 0,
          operations: []
        };
      }
  
      days[key].lastOperationAmount = operation.accountAmountAfterOperation;
      days[key].operations.push(operation);
    });
  
    return Object.keys(days).sort().map(key => ({
      day: key,
      lastOperationAmount: days[key].lastOperationAmount,
      operations: days[key].operations
    }));
  };

  const groupByWeek = (operations) => {
    const weeks = {};
  
    operations.forEach((operation) => {
      const date = new Date(operation.operationDate);
      const weekStart = startOfWeek(date, { weekStartsOn: 1 });
      const weekKey = weekStart.toISOString().split('T')[0];
  
      if (!weeks[weekKey]) {
        weeks[weekKey] = {
          lastOperationAmount: null,
          lastOperationDateTime: new Date(0) 
        };
      }
  
      
      if (date >= weeks[weekKey].lastOperationDateTime) {
        weeks[weekKey].lastOperationAmount = operation.accountAmountAfterOperation;
        weeks[weekKey].lastOperationDateTime = date;
      }
    });
  
    
    return Object.keys(weeks).sort().reverse().map(weekKey => ({
      week: weekKey,
      lastOperationAmount: weeks[weekKey].lastOperationAmount
    }));
  };
    
  const groupByMonth = (operations) => {
    const months = {};
  
    operations.forEach((operation) => {
      const date = new Date(operation.operationDate);
      const monthStart = startOfMonth(date);
      const monthKey = monthStart.toISOString().split('T')[0].substring(0, 7); 
  
      if (!months[monthKey]) {
        months[monthKey] = {
          lastOperationAmount: null,
          lastOperationDateTime: new Date(0) 
        };
      }
  
     
      if (date >= months[monthKey].lastOperationDateTime) {
        months[monthKey].lastOperationAmount = operation.accountAmountAfterOperation;
        months[monthKey].lastOperationDateTime = date;
      }
    });
  
   
    return Object.keys(months).sort().reverse().map(monthKey => ({
      month: monthKey,
      lastOperationAmount: months[monthKey].lastOperationAmount
    }));
  };
    

    useEffect(() => {
      const fetchOperations = async () => {
        try {
          const response = await axios.get('https://localhost:7159/Controllers');
          const allOperations = response.data.filter(operation => operation.operationAmount !== 0);
          setOperations(allOperations);
  
          
  
          
          const dailyData = groupByDay(allOperations);
          const weeklyData = groupByWeek(allOperations);
          const monthlyData = groupByMonth(allOperations);
          
          console.log(weeklyData);

          const totalSpent = allOperations
      .filter(op => op.operationAmount < 0)
      .reduce((sum, op) => sum + Math.abs(op.operationAmount), 0);

    const totalEarned = allOperations
      .filter(op => op.operationAmount > 0)
      .reduce((sum, op) => sum + op.operationAmount, 0);

    const averageTransaction = allOperations.length > 0
      ? allOperations.reduce((sum, op) => sum + op.operationAmount, 0) / allOperations.length
      : 0;

    const largestTransaction = Math.max(...allOperations.map(op => Math.abs(op.operationAmount)));

    const totalTransactions = allOperations.length;

    setStats({
      totalSpent,
      totalEarned,
      averageTransaction,
      largestTransaction,
      totalTransactions
    });

          setDailyChartData({
            labels: dailyData.map(data => data.day),
            datasets: [{
              label: 'Saldo po ostatniej operacji (dzień)',
              data: dailyData.map(data => data.lastOperationAmount),
              backgroundColor: 'rgba(53, 162, 235, 0.5)',
              borderColor: 'rgba(53, 162, 235, 1)',
              borderWidth: 1,
            }],
          });

          
          setWeeklyChartData({
            labels: weeklyData.map(data => data.week),
            datasets: [{
              label: 'Saldo po ostatniej operacji (tydzień)',
              data: weeklyData.map(data => data.lastOperationAmount),
              backgroundColor: 'rgba(75, 250, 15, 0.5)',
              borderColor: 'rgba(75, 250, 15, 1)',
              borderWidth: 1,
            }],
          });
          
          
          setMonthlyChartData({
            labels: monthlyData.map(data => data.month), 
            datasets: [{
              label: 'Saldo po operacji (miesiąc)',
              data: monthlyData.map(data => data.lastOperationAmount),
              backgroundColor: 'rgba(153, 102, 255, 0.5)',
              borderColor: 'rgba(153, 102, 255, 1)',
              borderWidth: 1,
            }],
          });
  
        } catch (error) {
          console.error('Error fetching data: ', error);
        }
      };
  
      fetchOperations();
    }, []); 

  
  const getAmountDetails = (amount) => {
    return amount < 0
      ? { text: "Obciążenie", color: 'red' }
      : { text: "Uznanie", color: 'green' };
  };

  return (
    <Container sx={{ maxWidth: '100%', my: 1, mx: 1 }}>
       <Grid container spacing={3} justifyContent="start" alignItems="space-between">
        <Grid item xs={12} md={4} >
        <Typography variant="h2" component="h1" gutterBottom sx={{ textAlign: 'left' }}>
            Operacje
          </Typography>
          <Paper elevation={3} sx={{ backgroundColor: '#f5deb3', maxHeight: '80vh', overflow: 'auto' }}>
          <List>
          {operations.map(operation => {
            const amountDetails = getAmountDetails(operation.operationAmount);
            return (
              <ListItem key={operation.id} divider sx={{ justifyContent: 'flex-start' }}>
                <ListItemText
                  primary={
                    <>
                      Data: {new Date(operation.operationDate).toLocaleDateString()},
                      <span style={{ color: amountDetails.color }}> {amountDetails.text}</span>,
                      Kwota: {operation.operationAmount}
                    </>
                  }
                  secondary={`Opis: ${operation.description}`}
                />
              </ListItem>
            );
          })}
        </List>
          </Paper>
          </Grid>
        <Grid item xs={12} md={4}>
          <Typography variant="h4" component="h2" gutterBottom>
            Dzienny Bilans
          </Typography>
          <Line data={dailyChartData} />

          <Typography variant="h4" component="h2" gutterBottom sx={{ marginTop: 4 }}>
            Tygodniowy Bilans
          </Typography>
          <Line data={weeklyChartData} />

          <Typography variant="h4" component="h2" gutterBottom>
            Misesięczny Bilans
          </Typography>
          <Line data={monthlyChartData} />
        </Grid>

        <Grid item xs={12} md={4}>
        <Typography variant="h4" component="h3" gutterBottom>
          Statystyki
        </Typography>
        <Paper elevation={3} sx={{ backgroundColor: '#f5deb3', maxHeight: '80vh', overflow: 'auto' }}>
          <Box sx={{ padding: 2 }}>
            <Typography>Całkowite wydatki: {stats.totalSpent.toFixed(2)}</Typography>
            <Typography>Całkowite przychody: {stats.totalEarned.toFixed(2)}</Typography>
            <Typography>Największa transakcja: {stats.largestTransaction.toFixed(2)}</Typography>
            <Typography>Liczba transakcji: {stats.totalTransactions}</Typography>
          </Box>
        </Paper>
      </Grid>
      </Grid>
    </Container>
  );
};

export default App;










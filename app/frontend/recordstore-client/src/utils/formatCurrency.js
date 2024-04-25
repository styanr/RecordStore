const formatCurrency = (value) => {
  return new Intl.NumberFormat('uk-UA', {
    style: 'currency',
    currency: 'UAH',
  }).format(value);
};

export default formatCurrency;

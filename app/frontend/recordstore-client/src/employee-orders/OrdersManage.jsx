import { Box, Container, Heading } from '@chakra-ui/react';

import OrderSection from '../profile/OrderSection';

import useOrders from '../order/useOrders';

const OrdersManage = () => {
  const { orders, statusOptions, fetchOrders, updateOrderStatus } =
    useOrders('employee');

  return (
    <Box bg='gray.100' minH='100vh' py={12}>
      <Container maxW='7xl'>
        <Heading>Керування замовленнями</Heading>
        <OrderSection
          orders={orders}
          fetchOrders={fetchOrders}
          manage
          statusOptions={statusOptions}
          updateStatus={updateOrderStatus}
        />
      </Container>
    </Box>
  );
};

export default OrdersManage;
